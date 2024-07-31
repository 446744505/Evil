namespace Edb
{
    public interface ILoggerEngine : IDisposable
    {
        public Task BeforeFlush();
        public Task AfterFlush(bool success);
        public void Backup(string path, bool increment);
        public void DropTables(string[] tableNames);
    }
}