namespace Edb
{
    public interface IStorageInterface<TKey> where TKey : notnull
    {
        public IStorageEngine<TKey> Engine { get; }
        public long MarshalN();
        public long Marshal0();
        public long Snapshot();
        public Task<long> Flush0Async();
        public void Cleanup();

        public async Task<long> Flush1Async()
        {
            var count = await Flush0Async();
            Cleanup();
            return count;
        }
    }
}