using System;
using System.IO;
using System.Threading.Tasks;
using NetWork.Proto;
using NetWork.Util;
using ProtoBuf;

namespace NetWork
{
    public abstract class Rpc<T> : Message where T : Message
    {
        private long m_RequestId = IdGenerator.NextId();
        private TaskCompletionSource<T> m_TaskCompletionSource;

        public async Task<T> SendAsync(Session? session, int timeout = 10000)
        {
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }

            await session.Send(this);
            m_TaskCompletionSource = new TaskCompletionSource<T>();
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => m_TaskCompletionSource.TrySetCanceled());
            await Task.WhenAny(timeoutTask, m_TaskCompletionSource.Task);
            try
            {
                return await m_TaskCompletionSource.Task;
            } catch (TaskCanceledException)
            {
                throw new TimeoutException(ToString()!);
            }
        }

        public override void Process()
        {
            var result = DeRequest();
            var rsp = new RpcResponse() { RequestId = m_RequestId };
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, result);
                rsp.Data = stream.ToArray();
            }

            Session.Send(rsp);
        }

        public override void Encode(BinaryWriter writer)
        {
            base.Encode(writer);
            writer.Write(m_RequestId);
        }
        
        public override void Decode(BinaryReader reader)
        {
            base.Decode(reader);
            m_RequestId = reader.ReadInt64();
        }

        public virtual T DeRequest()
        {
            throw new NotImplementedException();
        }
    }
}