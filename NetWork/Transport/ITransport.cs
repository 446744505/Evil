using System;
using System.Threading.Tasks;

namespace NetWork.Transport
{
    public interface ITransport : IDisposable
    {
        Task Start();
    }
}