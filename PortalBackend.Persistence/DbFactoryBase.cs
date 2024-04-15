using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AccessHomes.Persistence
{
    public abstract class DbFactoryBase
    {
        private readonly IConfiguration _config;

        internal string DbConnectionString => _config.GetConnectionString("SQLDBConnectionString");

        public DbFactoryBase(IConfiguration config)
        {
            _config = config;
        }

        internal IDbConnection DbConnection => new OracleConnection(DbConnectionString);

        public virtual async Task<IEnumerable<T>> DbQueryAsync<T>(string sql, object parameters = null)
        {
            using IDbConnection dbCon = DbConnection;
            return parameters == null ? await dbCon.QueryAsync<T>(sql) : await dbCon.QueryAsync<T>(sql, parameters);
        }
        public virtual async Task<T> DbQuerySingleAsync<T>(string sql, object parameters)
        {
            using IDbConnection dbCon = DbConnection;
            return await dbCon.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        public virtual async Task<bool> DbExecuteAsync<T>(string sql, object parameters)
        {
            using IDbConnection dbCon = DbConnection;
            return await dbCon.ExecuteAsync(sql, parameters) > 0;
        }

        public virtual async Task<bool> DbExecuteScalarAsync(string sql, object parameters)
        {
            using IDbConnection dbCon = DbConnection;
            return await dbCon.ExecuteScalarAsync<bool>(sql, parameters);
        }

        public virtual async Task<T> DbExecuteScalarDynamicAsync<T>(string sql, object parameters = null)
        {
            using IDbConnection dbCon = DbConnection;
            return parameters == null ? await dbCon.ExecuteScalarAsync<T>(sql) : await dbCon.ExecuteScalarAsync<T>(sql, parameters);
        }
    }
}
