using System.Runtime.CompilerServices;

namespace Evil.Util
{

    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}