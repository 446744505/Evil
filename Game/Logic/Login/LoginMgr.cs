using Evil.Util;
using Proto;

namespace Login
{
    public class LoginMgr : Singleton<LoginMgr>
    {
        public readonly LoginService LoginService = new();
    }
}