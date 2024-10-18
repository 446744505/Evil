
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Evil.Util;
using ProtoBuf;

namespace NetWork
{
    public abstract class RpcAck : Message
    {
        public const int Success = 0;
        // 内部错误码用负数
        public const int Timeout = -1;
        public const int Exception = -2;
        public const int SessionIsNull = -3;

        public int Code { get; set; } = 0;
    }
    
    public abstract class Rpc<T> : Message where T : RpcAck
    {
        private static readonly Lazy<T> SessionIsNull = new (() => CreateAck(RpcAck.SessionIsNull));
        private static readonly Lazy<T> Timeout = new (() => CreateAck(RpcAck.Timeout));
        private static readonly Lazy<T> Exception = new (() => CreateAck(RpcAck.Exception));

        private static readonly Exception NotImplementException = new NotImplementedException();
        
        private long m_RequestId = IdGenerator.NextId();

        public static T CreateAck(int code)
        {
            var type = typeof(T);
            var ack = (T)Activator.CreateInstance(type)!;
            ack.Code = code;
            return ack;
        }

        public async Task<T> SendAsync(Session? session, int timeout = 5000)
        {
            if (session == null)
            {
                return SessionIsNull.Value;
            }

            try
            {
                var rpcMgr = session.Transport.RpcMgr();
                var completionSource = new TaskCompletionSource<T>();
                var cts = new CancellationTokenSource();
                // 这里直接用了Task.Delay(用的后台线程),不用担心停服时超时任务不会执行
                // 因为在RpcMgr关闭时会等待所有的PendingRequest执行完,会hold住主线程
                var timeoutTask = Task.Delay(timeout, cts.Token).ContinueWith(_ => completionSource.TrySetCanceled());
                rpcMgr.PendRequest(m_RequestId, stream =>
                {
                    var message = Serializer.NonGeneric.Deserialize(typeof(T), stream) as T;
                    MessageHelper.OnReceiveMsg(session, message!);
                    completionSource.TrySetResult(message!);
                    cts.Cancel(); // 取消超时定时器
                });

                session.Send(this);
                await Task.WhenAny(timeoutTask, completionSource.Task);

                try
                {
                    return await completionSource.Task;
                }
                catch (TaskCanceledException)
                {
                    rpcMgr.RemovePending(m_RequestId);
                    return Timeout.Value;
                }
            }
            catch (Exception e)
            {
                Log.I.Error($"rpc {this} process exception", e);
                return Exception.Value;
            }
        }

        public override bool Process()
        {
            var result = OnRequest();
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

            Session.Send(rsp);
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

        public virtual T OnRequest()
        {
            throw NotImplementException;
        }
    }
}