namespace Edb
{
    public abstract class TTable<TKey, TValue> : BaseTable<TKey> where TKey : notnull
    {
        internal void OnRecordChanged(TRecord<TKey, TValue> r, LogNotify ln)
        {
            
        }
        
        public abstract TKey MarshalKey(TKey key);
        public abstract object MarshalValue(TValue value);
    }
}