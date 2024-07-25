namespace Edb
{
    public interface IStorage
    {
        public Task<long> MarshalN();
        public long Marshal0();
        public long Snapshot();
        public Task<long> Flush0Async();
        public Task Cleanup();

        public async Task<long> Flush1Async()
        {
            var count = await Flush0Async();
            await Cleanup();
            return count;
        }
    }
}