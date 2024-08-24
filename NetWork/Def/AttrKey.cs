using DotNetty.Common.Utilities;

namespace NetWork
{
    public class AttrKey
    {
        public static readonly AttributeKey<Session> Session = AttributeKey<Session>.NewInstance("Session");
    }
}