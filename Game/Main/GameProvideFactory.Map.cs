using System.Collections.Generic;
using Evil.Provide;
using Proto;

namespace Game
{
    public partial class GameProvideFactory
    {
        /// <summary>
        /// 记录所有的map pvid
        /// </summary>
        /// <param name="providerUrl"></param>
        /// <param name="newAll"></param>
        private void OnProvideUpdateMap(string providerUrl, Dictionary<ushort,ProvideInfo> newAll)
        {
            foreach (var pair in newAll)
            {
                if (pair.Value.type == (int)ProvideType.Map)
                    Program.Ctx.AddMapPvid(pair.Key);
            }
        }
    }
}