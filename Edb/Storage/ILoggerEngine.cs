namespace Edb
{
    public interface ILoggerEngine : IDisposable
    {
        public void BeforeFlush();
        public void AfterFlush(bool success);
        public void Backup(string path, bool increment);
        public void DropTables(string[] tableNames);
    }
}