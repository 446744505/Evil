using System;

namespace NetWork.Transport
{
    public interface ITransport : IDisposable
    {
        void Start();
    }
}