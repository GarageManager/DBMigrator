using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;

namespace DBMigrator
{
    public class DbCluster
    {
        private readonly string _connectionString;
        private readonly string _providerName;

        public DbCluster(string providerName, string connectionString)
        {
            _providerName = providerName;
            _connectionString = connectionString;
        }

        public async Task InsertAsync<T>(object obj) where T : class
        {
            await using var db = new DataConnection(_providerName, _connectionString);
            var _= db.InsertAsync((T) obj).Result;
        }

        public async Task UpdateAsync<T>(object obj) where T : class
        {
            await using var db = new DataConnection(_providerName, _connectionString);
            await db
                .UpdateAsync((T) obj)
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(object key)
        {
            await using var db = new DataConnection(_providerName, _connectionString);
            await db
                .DeleteAsync(key)
                .ConfigureAwait(false);
        }

        public async Task<T[]> GetAllAsync<T>() where T : class
        {
            await using var db = new DataConnection(_providerName, _connectionString);
            return await db.GetTable<T>()
                .ToArrayAsync()
                .ConfigureAwait(false);
        }

        public async Task<T[]> GetAllAsync<T, TKey>(int offset, int batchSize, Expression<Func<T, TKey>> order)
            where T : class
        {
            await using var db = new DataConnection(_providerName, _connectionString);
            return db.GetTable<T>()
                .OrderBy(order)
                .Skip(offset)
                .Take(batchSize)
                .ToArrayAsync()
                .Result;
        }

        public async Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            await using var db = new DataConnection(_providerName, _connectionString);
            return db.GetTable<T>()
                .FirstOrDefaultAsync(predicate)
                .Result;
        }

        public async Task<int> InsertOrUpdateAsync<T>(Expression<Func<T>> insertSetter,
            Expression<Func<T, T>> onDuplicateKeyUpdateSetter) where T : class
        {
            await using var db = new DataConnection(_providerName, _connectionString);
            return await db.GetTable<T>()
                .InsertOrUpdateAsync(insertSetter, onDuplicateKeyUpdateSetter)
                .ConfigureAwait(false);
        }
    }
}