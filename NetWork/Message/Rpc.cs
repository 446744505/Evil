using System.IO;
using System.Threading.Tasks;
using NetWork.Util;

namespace NetWork
{
    public class Rpc<T> : Message
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
            await Task.Delay(timeout).ContinueWith(_ => m_TaskCompletionSource.TrySetException(new TimeoutException(ToString()!)));
            return await m_TaskCompletionSource.Task;
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
    }
}