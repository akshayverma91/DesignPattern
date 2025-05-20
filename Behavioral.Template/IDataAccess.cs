using System.Data.Common;

namespace Behavioral.Template
{
    public interface IDataAccess<T>
    {
        Task<T> GetAsync(string id, bool useWriteConnection = false);
        Task<T> CreateAsync(T item);
        Task<T> UpdateAsync(T item);

        string GetIdColumn();
        string GetSelect();
        string GetFrom();
        string GetTableName();
        Task<T> GetItemFromReaderAsync(DbDataReader reader);
        Task<Dictionary<string, object>> GetFieldValuesAsync(T item);
        Dictionary<string, MapAndType> GetMappings();
    }
}
