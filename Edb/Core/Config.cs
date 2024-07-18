namespace Edb
{
    public class Config
    {
        public bool Verify { get; set; } = true;
        public string DbUrl { get; set; } = "mongodb://localhost:27017";
        public string DbName { get; set; } = "edb_test";
    }
}