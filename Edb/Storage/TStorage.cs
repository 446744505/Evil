
namespace Edb
{
    internal class TStorage<TKey, TValue> : IStorageInterface<TKey> 
        where TKey : notnull where TValue : class
    {
        private readonly TTable<TKey, TValue> m_Table;
        public IStorageEngine<TKey> Engine { get; }
        
        public TStorage(TTable<TKey, TValue> table, ILoggerEngine logger)
        {
            m_Table = table;
            Engine = new StorageMongo<TKey>((LoggerMongo)logger, table.Name);
        }
        
        public long MarshalN()
        {
            throw new NotImplementedException();
        }

        public long Marshal0()
        {
            throw new NotImplementedException();
        }

        public long Snapshot()
        {
            throw new NotImplementedException();
        }

        public long Flush0()
        {
            throw new NotImplementedException();
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }
    }
}