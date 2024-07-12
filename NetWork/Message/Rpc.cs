using System;
using System.Collections.Concurrent;
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

        public async Task<T> SendAsync(Session? session, int timeout = 10000)
        {
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }

            await session.Send(this);
            var completionSource = new TaskCompletionSource<T>();
            RpcMgr.I.PendRequest(m_RequestId, completionSource);
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => completionSource.TrySetCanceled());
            await Task.WhenAny(timeoutTask, completionSource.Task);
            try
            {
                return await completionSource.Task;
            } catch (TaskCanceledException)
            {
                RpcMgr.I.RemovePending(m_RequestId);
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
                rsp.Data = stream.GetBuffer()[..(int)stream.Length];
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