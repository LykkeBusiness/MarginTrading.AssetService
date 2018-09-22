using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.SqlRepositories.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly string _tableName;
        private const string CreateTableScript = "CREATE TABLE [{0}](" +
                                                 "[Id] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                                                 "[DateTime] [DateTime] NOT NULL," +
                                                 "[Level] [nvarchar] (64) NOT NULL, " +
                                                 "[Env] [nvarchar] (64) NULL, " +
                                                 "[AppName] [nvarchar] (256) NULL, " +
                                                 "[Version] [nvarchar] (256) NULL, " +
                                                 "[Component] [nvarchar] (1024) NULL, " +
                                                 "[Process] [nvarchar] (1024) NOT NULL, " +
                                                 "[Context] [nvarchar] (MAX) NOT NULL, " +
                                                 "[Type] [nvarchar] (1024) NOT NULL, " +
                                                 "[Stack] [nvarchar] (MAX) NULL, " +
                                                 "[Msg] [nvarchar] (MAX) NULL " +
                                                 ");";
        
        private static Type DataType => typeof(ILogEntity);
        private static readonly string GetColumns = "[" + string.Join("],[", DataType.GetProperties().Select(x => x.Name)) + "]";
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));

        private readonly string _connectionString;
        
        public LogRepository(string logTableName, string connectionString)
        {
            _tableName = logTableName;
            _connectionString = connectionString;
            
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.CreateTableIfDoesntExists(CreateTableScript, _tableName);
            }
        }
        
        public async Task Insert(ILogEntity log)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"insert into {_tableName} ({GetColumns}) values ({GetFields})", log);
            }
        }
    }
}