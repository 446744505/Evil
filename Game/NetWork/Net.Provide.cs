using System.Threading.Tasks;
using Evil.Provide;
using NetWork;

namespace Game.NetWork
{
    public partial class Net
    {
        internal Provide Provide { get; set; }
        
        public void SendToProvide(ushort pvid, Message msg)
        {
            Provide.SendToProvide(pvid, msg);
        }
        
        public async Task<T?> SendToProvideAsync<T>(ushort pvid, Rpc<T> rpc) where T : Message
        {
            return await Provide.SendToProvideAsync(pvid, rpc);
        }
    }
}