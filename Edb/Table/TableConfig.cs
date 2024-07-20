namespace Edb
{
    public class TableConfig
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Lock { get; set; }
        public int CacheCapacity { get; set; }
        public bool IsMemory { get; set; }
    }
}