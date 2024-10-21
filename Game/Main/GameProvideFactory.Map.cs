using System.Collections.Generic;
using Evil.Provide;
using Game.NetWork;
using Proto;

namespace Game
{
    public partial class GameProvideFactory
    {
        /// <summary>
        /// 更新所有可用的map pvid
        /// </summary>
        /// <param name="providerUrl"></param>
        /// <param name="newAll"></param>
        private void OnProvideUpdateMap(
            string providerUrl,
            Dictionary<ushort, ProvideInfo> newAll, 
            List<ProvideInfo> added, 
            List<ProvideInfo> removed)
        {
            foreach (var info in newAll.Values)
            {
                var provide = Net.I.Provide;
                if (info.type == (int)ProvideType.Map)
                {
                    var pvid = (ushort)info.pvid;
                    if (provide.IsSelfLinkProvide(providerUrl, pvid))
                    {
                        Program.Ctx.AddMapPvid(pvid);
                    }
                }
            }

            foreach (var info in removed)
            {
                Program.Ctx.RemoveMapPvid((ushort)info.pvid);
            }
        }
    }
}