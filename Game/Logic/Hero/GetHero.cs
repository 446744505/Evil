namespace Logic.Hero.Proto
{
    public partial class GetHero
    {
        public override Hero DeRequest()
        {
            return new Hero()
            {
                heroId = heroId,
                Star = 32,
            };
        }
    }
}