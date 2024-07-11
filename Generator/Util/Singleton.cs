using System;

namespace Generator.Util
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> Instance = new(() => new T());
        
        private protected Singleton() { }
        
        public static T I => Instance.Value;
    }
}