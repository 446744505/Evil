using Generator.Context;

namespace Generator
{
    public partial class Progress
    {
        public void CreateFile(FileContext? fc)
        {
            fc?.CreateFile();
        }
    }
}