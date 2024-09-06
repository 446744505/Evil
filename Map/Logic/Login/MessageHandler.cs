using Evil.Util;
using Map;

namespace Proto
{
    public partial class LoginToMap
    {
        public override async Task<LoginToMapAck> OnRequest()
        {
            Log.I.Info($"player {playerId} login");
            return new LoginToMapAck()
            {
                data = CmdLine.I.Pvid, 
            };
        }
    }
}