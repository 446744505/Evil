namespace Edb
{
    public abstract class TTable<TKey, TValue> : BaseTable<TKey, TValue> where TKey : notnull
    {
        internal void OnRecordChanged(TRecord<TKey, TValue> r, LogNotify ln)
        {
            
        }
    }
}