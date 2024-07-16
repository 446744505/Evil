
namespace Evil.Util
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> Instance = new(() => new T());
        public static T I => Instance.Value;
    }
}