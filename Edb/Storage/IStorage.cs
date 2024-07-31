namespace Edb
{
    public interface IStorage
    {
        public Task<long> MarshalN();
        public long Marshal0();
        public long Snapshot();
        public Task<long> FlushAsync();
        public Task Cleanup();
    }
}