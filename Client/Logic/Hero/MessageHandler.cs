using System.Threading.Tasks;
using Evil.Util;

namespace Proto
{
    public partial class HeroStarNtf
    {
        public override Task<bool> Process()
        {
            Log.I.Info($"hero star ntf {heroId} star:{star}");
            return TrueTask;
        }
    }
}