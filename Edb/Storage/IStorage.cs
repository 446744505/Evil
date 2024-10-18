namespace Edb
{
    public interface IStorage
    {
        public long MarshalN();
        public long Marshal0();
        public long Snapshot();
        public long FlushAsync();
        public void Cleanup();
    }
}