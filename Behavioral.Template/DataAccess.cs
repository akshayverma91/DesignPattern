using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Behavioral.Template
{
    public interface ICommandProvider
    {
        DbCommand CreateCommand(string sql, DbConnection connection);
    }
    public interface IDatabase
    {
        DbConnection GetConnection(bool useWriteConnection);
    }
    //template Abstract Class
    public abstract class DataAccess<T>: IDataAccess<T>
    {
        protected readonly IDatabase _db;
        protected readonly ICommandProvider _cmdProvider;

        protected DataAccess(IDatabase db, ICommandProvider cmdProvider)
        {
            _db = db;
            _cmdProvider = cmdProvider;
        }

        // Template method for getting an entity by ID
        public virtual async Task<T> GetAsync(string id, bool useWriteConnection = false)
        {
            var sql = $"{GetSelect()} {GetFrom()} WHERE {GetIdColumn()} = @id";
            using (var cmd = _cmdProvider.CreateCommand(sql, _db.GetConnection(useWriteConnection)))
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "@id";
                parameter.Value = id;
                cmd.Parameters.Add(parameter);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return await GetItemFromReaderAsync(reader);
                    }
                }
            }
            return default;
        }

        // Template method for creating an entity
        public virtual async Task<T> CreateAsync(T item)
        {
            var values = await GetFieldValuesAsync(item);
            var sql = BuildInsertSql(values);
            using (var cmd = _cmdProvider.CreateCommand(sql, _db.GetConnection(true)))
            {
                foreach (var kvp in values)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "@" + kvp.Key;
                    parameter.Value = kvp.Value ?? DBNull.Value;
                    // Optionally set parameter.DbType here if you know the type
                    cmd.Parameters.Add(parameter);
                }

                await cmd.ExecuteNonQueryAsync();
            }
            // Optionally, fetch and return the created entity
            // (Assumes item has an Id property set after insert)
            return item;
        }

        // Template method for updating an entity
        public virtual async Task<T> UpdateAsync(T item)
        {
            var values = await GetFieldValuesAsync(item);
            var sql = BuildUpdateSql(values);
            using (var cmd = _cmdProvider.CreateCommand(sql, _db.GetConnection(true)))
            {
                foreach (var kvp in values)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "@" + kvp.Key;
                    parameter.Value = kvp.Value ?? DBNull.Value;
                    // Optionally set parameter.DbType here if you know the type
                    cmd.Parameters.Add(parameter);
                }

                await cmd.ExecuteNonQueryAsync();
            }
            return item;
        }

        // --- Methods to be implemented/overridden by subclasses ---

        public abstract string GetIdColumn();
        public abstract string GetSelect();
        public abstract string GetFrom();
        public abstract string GetTableName();
        public abstract Task<T> GetItemFromReaderAsync(DbDataReader reader);
        public abstract Task<Dictionary<string, object>> GetFieldValuesAsync(T item);
        public abstract Dictionary<string, MapAndType> GetMappings();


        // --- Helper methods for SQL generation (simplified) ---

        protected virtual string BuildInsertSql(Dictionary<string, object> values)
        {
            var columns = string.Join(", ", values.Keys);
            var parameters = string.Join(", ", values.Keys.Select(k => "@" + k));
            //var parameters = string.Join(", ", values.Keys, 0, values.Count, k => "@" + k);
            return $"INSERT INTO {GetTableName()} ({columns}) VALUES ({parameters})";
        }

        protected virtual string BuildUpdateSql(Dictionary<string, object> values)
        {
            var setClause = string.Join(", ", values.Keys.Select(k => $"{k} = @{k}"));
            return $"UPDATE {GetTableName()} SET {setClause} WHERE {GetIdColumn()} = @id";
        }
    }
}