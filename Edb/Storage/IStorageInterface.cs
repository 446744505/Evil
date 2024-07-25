namespace Edb
{
    public interface IStorageInterface<TKey> : IStorage where TKey : notnull
    {
        public IStorageEngine<TKey> Engine { get; }
    }
}