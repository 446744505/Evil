namespace Edb
{
    public sealed partial class Transaction
    {
        private readonly Dictionary<LogKey, object> m_Wrappers = new();
        
        internal Dictionary<LogKey, object> Wrappers => m_Wrappers;
        

        private static readonly ThreadLocal<Transaction> ThreadLocal = new();
        internal static Transaction? Current => ThreadLocal.Value;
        

        internal static Transaction Create()
        {
            var self = Current;
            if (self == null)
                ThreadLocal.Value = self = new Transaction();
            return self;
        }
    }
}