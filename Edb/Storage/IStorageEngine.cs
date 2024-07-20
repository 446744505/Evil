namespace Edb
{
    public interface IStorageEngine<TKey> where TKey : notnull
    {
        Task<bool> InsertAsync(object value);
        Task ReplaceAsync(TKey key, object value);
        Task<object?> FindAsync(TKey key);
        Task RemoveAsync(TKey key);
        Task<bool> ExistsAsync(TKey key);
        Task WalkAsync(Action<object> walker);
    }
}