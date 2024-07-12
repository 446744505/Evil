using System.Text;

namespace Generator.Util
{
    public class MessageIdGenerator
    {
        public static uint CalMessageId(string messageName)
        {
            return BKDRHash(messageName);
        }
        
        public static uint BKDRHash(string str)
        {
            return BKDRHash(Encoding.UTF8.GetBytes(str));
        }
        
        public static uint BKDRHash(byte[] bytes)
        {
            uint seed = 131;
            uint hash = 0;
            foreach (var b in bytes)
            {
                hash = hash * seed + b;
            }

            return hash;
        }
    }
}