using Evil.Util;

namespace Hero.Proto
{
    public partial class HeroStarNtf
    {
        public override void Process()
        {
            Log.I.Info($"heroId:{heroId} star:{Star}");
        }
    }
}