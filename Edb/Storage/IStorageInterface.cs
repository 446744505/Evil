namespace Edb
{
    public interface IStorageInterface<TKey> where TKey : notnull
    {
        public IStorageEngine<TKey> Engine { get; }
        public long MarshalN();
        public long Marshal0();
        public long Snapshot();
        public long Flush0();
        public void Cleanup();

        public long Flush1()
        {
            var count = Flush0();
            Cleanup();
            return count;
        }
    }
}