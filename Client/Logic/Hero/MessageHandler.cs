using Evil.Util;

namespace Proto
{
    public partial class HeroStarNtf
    {
        public override void Process()
        {
            Log.I.Info($"hero star ntf {heroId} star:{star}");
        }
    }
}