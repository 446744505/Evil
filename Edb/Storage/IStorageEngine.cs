namespace Edb
{
    public interface IStorageEngine<TKey, TValue> where TKey : notnull
    {
        Task<bool> InsertAsync(TValue value);
        Task ReplaceAsync(TKey key, TValue value);
        Task<TValue> FindAsync(TKey key);
        Task RemoveAsync(TKey key);
        Task<bool> ExistsAsync(TKey key);
        Task WalkAsync(Action<TValue> walker);
    }
}