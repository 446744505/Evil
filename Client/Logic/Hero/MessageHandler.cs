using System.Threading.Tasks;
using Evil.Util;

namespace Proto
{
    public partial class HeroStarNtf
    {
        public override bool Process()
        {
            Log.I.Info($"hero star ntf {heroId} star:{star}");
            return true;
        }
    }
}