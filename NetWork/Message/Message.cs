using System.IO;
using System.Threading.Tasks;
using Evil.Util;

namespace NetWork
{
    public abstract class Message
    {
        public virtual uint MessageId { get; } = 0;
        public ushort Pvid { get; set; }
        public Session Session { get; set; } = null!;
        public IMessgeDispatcher Dispatcher { get; set; } = null!;

        public void Send(Session? session)
        {
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }
            session.SendAsync(this);
        }
        
        /// <summary>
        /// 在网络线程中执行，保证顺序
        /// </summary>
        /// <returns></returns>
        public virtual void Dispatch()
        {
            // 因为外面没有await，所以这里要用ContinueWith打印异常
            Dispatcher.Dispatch(this).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Log.I.Error("Message.Dispatch", task.Exception);
                }
                return task.Result;
            });
        }

        /// <summary>
        /// 异步执行，不保证顺序
        /// </summary>
        public virtual Task<bool> Process()
        {
            return Task.FromResult(false);
        }

        public virtual void Decode(BinaryReader reader)
        {
            Pvid = reader.ReadUInt16();
        }
        
        public virtual void Encode(BinaryWriter writer)
        {
            writer.Write(Pvid);
        }
    }
}