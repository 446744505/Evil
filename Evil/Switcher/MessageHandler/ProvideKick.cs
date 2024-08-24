using Evil.Switcher;
using Evil.Util;

namespace Proto
{
    public partial class ProvideKick
    {
        public override Task Dispatch()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession != null)
            {
                lock (linkerSession)
                {
                    linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
                    if (linkerSession != null)
                    {
                        Linker.I.CloseSession(linkerSession, code).Wait();
                        Log.I.Info($"provide {Session} kick {linkerSession} reason {code}");
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}