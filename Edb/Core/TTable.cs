namespace Edb
{
    public abstract class TTable<TKey, TValue> : BaseTable
    {
        internal void OnRecordChanged(TRecord<TKey, TValue> r, LogNotify ln)
        {
            
        }
    }
}