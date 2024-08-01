namespace Edb
{
    public interface ITable
    {
        enum Persistence
        {
            Memory,
            Db
        }
        
        public string Name { get; }
        public Persistence PersistenceType { get; }

        Task LogNotify();
    }
}