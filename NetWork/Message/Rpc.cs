using System;
using System.IO;
using System.Threading.Tasks;
using Evil.Util;
using NetWork.Proto;
using ProtoBuf;

namespace NetWork
{
    public abstract class Rpc<T> : Message where T : Message
    {
        private long m_RequestId = IdGenerator.NextId();

        public async Task<T> SendAsync(Session? session, int timeout = 5000)
        {
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }

            await session.SendAsync(this);
            var completionSource = new TaskCompletionSource<T>();
            RpcMgr.I.PendRequest(m_RequestId, stream =>
            {
                var message = Serializer.NonGeneric.Deserialize(typeof(T), stream) as T;
                return completionSource.TrySetResult(message!);
            });
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

        public override async Task<bool> Process()
        {
            var result = await OnRequest();
            var rsp = new RpcResponse() { RequestId = m_RequestId };
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, result);
                rsp.Data = stream.GetBuffer()[..(int)stream.Length];
            }

            await Session.SendAsync(rsp);
            return true;
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

        public virtual Task<T> OnRequest()
        {
            throw new NotImplementedException();
        }
    }
}