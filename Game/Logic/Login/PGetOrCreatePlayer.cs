using System.Threading.Tasks;
using Edb;
using Evil.Util;
using XBean;

namespace Game.Logic.Login
{
    public class PGetOrCreatePlayer : RProcedure<long>
    {
        private readonly string m_Account;

        public PGetOrCreatePlayer(string account)
        {
            m_Account = account;
        }

        public override async Task<bool> Process()
        {
            // TODO 放到au里去
            var user = await XTable.User.Update(m_Account);
            if (user is null)
            {
                user = new User();
                user.Account = m_Account;
                await XTable.User.Insert(user);
                Log.I.Info($"create user {m_Account}");
            }

            if (user.PlayerIds.Count == 0)
            {
                // TODO 换成全局ID
                var playerId = IdGenerator.NextId();
                user.PlayerIds.Add(playerId);
                Log.I.Info($"user {m_Account} create player {playerId}");
            }

            R = user.PlayerIds[0];

            return true;
        }
    }
}