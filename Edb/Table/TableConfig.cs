namespace Edb
{
    public class TableConfig
    {
        public string Name { get; set; } = null!;
        public string Lock { get; set; } = null!;
        public int CacheCapacity { get; set; }
        public bool IsMemory { get; set; }
    }
}