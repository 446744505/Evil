namespace Edb
{
    public interface ILoggerEngine : IDisposable
    {
        public void Checkpoint();
        public void Backup(string path, bool increment);
        public void DropTables(string[] tableNames);
    }
}