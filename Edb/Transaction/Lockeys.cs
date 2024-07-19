
namespace Edb
{
    internal class Lockeys
    {
        private readonly int m_BucketShift;
        private readonly Bucket[] m_Buckets;

        private static readonly Lockeys I = new();
        
        private Lockeys()
        {
            var shift = Environment.GetEnvironmentVariable("edb.bucketShift");
            m_BucketShift = shift == null ? 10 : int.Parse(shift);
            m_Buckets = new Bucket[1 << m_BucketShift];
            for (var i = 0; i < m_Buckets.Length; i++)
            {
                m_Buckets[i] = new Bucket();
            }
        }

        public static Lockey GetLockey(int lockId, object key)
        {
            return I.Get(new Lockey(lockId, key));
        }

        private Lockey Get(Lockey lockey)
        {
            var transaction = Transaction.Current;
            if (transaction != null)
            {
                var lockey0 = transaction.Get(lockey);
                if (lockey0 != null)
                {
                    return lockey0;
                }
            }
            var h = lockey.GetHashCode();
            h += (int)((h << 15) ^ 0xffffcd7d);
            h ^= (h >>> 10);
            h += (h << 3);
            h ^= (h >>> 6);
            h += (h << 2) + (h << 14);
            h = (h ^ (h >>> 16)) >>> (32 - m_BucketShift);
            return m_Buckets[h].Get(lockey);
        }
        
        private class Bucket
        {
            private readonly Entry m_Head = new(null!, null!);
            private class Entry
            {
                public WeakReference<Lockey> m_Ref;
                public Entry m_Next;
                public Entry(Lockey referent, Entry next)
                {
                    m_Ref = new WeakReference<Lockey>(referent);
                    m_Next = next;
                }
            }

            public Lockey Get(Lockey lockey)
            {
                lock(this)
                {
                    var e = m_Head;
                    while (e.m_Next != null)
                    {
                        if (e.m_Next.m_Ref.TryGetTarget(out var key))
                        {
                            if (key.Equals(lockey))
                            {
                                return key;
                            }
                            else
                            {
                                e = e.m_Next;
                            }
                        }
                        else
                        {
                            e.m_Next = e.m_Next.m_Next;
                        }
                    }
                    e.m_Next = new Entry(lockey, null!);
                    return lockey.Alloc();
                }
            }
        }
    }
}