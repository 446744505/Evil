
using System;
using System.IO;
using System.Threading.Tasks;
using Evil.Util;
using ProtoBuf;

namespace NetWork
{
    public abstract class Rpc<T> : Message where T : Message
    {
        private static readonly Task<T> NotImplementException = Task.FromException<T>(new NotImplementedException());
        
        private long m_RequestId = IdGenerator.NextId();

        public async Task<T> SendAsync(Session? session, int timeout = 5000)
        {
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }

            var rpcMgr = session.Transport.RpcMgr();
            var completionSource = new TaskCompletionSource<T>();
            rpcMgr.PendRequest(m_RequestId, stream =>
            {
                var message = Serializer.NonGeneric.Deserialize(typeof(T), stream) as T;
                MessageHelper.OnReceiveMsg(session, message!);
                return completionSource.TrySetResult(message!);
            });
            
            await session.SendAsync(this);
            
            // 这里直接用了Task.Delay(用的后台线程),不用担心停服时超时任务不会执行
            // 因为在RpcMgr关闭时会等待所有的PendingRequest执行完,会hold住主线程
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => completionSource.TrySetCanceled());
            await Task.WhenAny(timeoutTask, completionSource.Task);
            try
            {
                return await completionSource.Task;
            } catch (TaskCanceledException)
            {
                rpcMgr.RemovePending(m_RequestId);
                throw new TimeoutException(ToString()!);
            }
        }

        public override async Task<bool> Process()
        {
            var result = await OnRequest();
            Message? rsp;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, result);
                var data = stream.GetBuffer()[..(int)stream.Length];
                rsp = Session.Config.NetWorkFactory!.CreateRpcResponse(Context, m_RequestId, data);
                if (rsp == null)
                {
                    throw new NotSupportedException();
                }
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
            return NotImplementException;
        }
    }
}