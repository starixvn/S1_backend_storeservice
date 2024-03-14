using System.Data;
using System.Diagnostics;

using Dapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;

using Serilog.Context;

using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Library;

public class MySqlClient
{
    public DataBase DataBase { get; set; }

    private readonly string _connectionString;

    private readonly IEnumerable<string> _backupConnectionStringList;

    private readonly ILogger<MySqlClient> _logger;

    public MySqlClient(IServiceProvider provider, DataBase dataBase, string connectionString)
    {
        DataBase = dataBase;
        _logger = provider.GetRequiredService<ILogger<MySqlClient>>();
        _connectionString = connectionString;
        _backupConnectionStringList = null;
    }

    public MySqlClient(IServiceProvider provider, DataBase dataBase, string connectionString, IEnumerable<string> backupConnectionStringList)
    {
        DataBase = dataBase;
        _logger = provider.GetRequiredService<ILogger<MySqlClient>>();
        _connectionString = connectionString;
        _backupConnectionStringList = backupConnectionStringList;
    }

    private string HealthCheck()
    {
        if (_backupConnectionStringList == null)
        {
            return _connectionString;
        }

        foreach (var backupConn in _backupConnectionStringList)
        {
            try
            {
                using var connection = new MySqlConnection(backupConn);
                var dbSlaveStatus = connection.QueryFirstOrDefault<DBSlaveStatus>("Service_HealthCheck_V2", commandType: CommandType.StoredProcedure);

                // 拿不到Status或延遲太久都換下一個連線
                if (dbSlaveStatus == null || dbSlaveStatus.Seconds_Behind_Master > 180)
                {
                    _logger.LogWarning($"{ReplaceDBString(backupConn)} | DelayTime = {dbSlaveStatus?.Seconds_Behind_Master}");
                    continue;
                }

                return backupConn;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{ReplaceDBString(backupConn)} | HealthCheck Exception");
            }
        }

        // 前面連線檢查都有問題，用最後一個連線
        return _backupConnectionStringList.Last();
    }

    public MySqlConnection GetMySqlConnection()
    {
        var conn = HealthCheck();
        return new MySqlConnection(conn);
    }

    public IEnumerable<T> Query<T>(string spName, object parameters = null, int? commandTimeout = null)
    {
        IEnumerable<T> result;
        var sw = Stopwatch.StartNew();
        try
        {
            var conn = HealthCheck();
            using var connection = new MySqlConnection(conn);
            result = connection.Query<T>(spName, parameters, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
            connection.Close();
            connection.Dispose();
        }
        catch (Exception e)
        {
            _logger.LogError(e, spName);
            throw;
        }

        LogContext.PushProperty("elapsed", sw.Elapsed.TotalMilliseconds);
        _logger.LogDebug($"{spName} {sw.Elapsed:c}");
        return result;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string spName, object parameters = null, int? commandTimeout = null)
    {
        IEnumerable<T> result;
        var sw = Stopwatch.StartNew();
        try
        {
            var conn = HealthCheck();
            await using var connection = new MySqlConnection(conn);
            result = await connection.QueryAsync<T>(spName, parameters, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, spName);
            throw;
        }

        LogContext.PushProperty("elapsed", sw.Elapsed.TotalMilliseconds);
        _logger.LogDebug($"{spName} {sw.Elapsed:c}");
        return result;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string spName, IDbTransaction transaction, object parameters = null)
    {
        IEnumerable<T> result;
        var sw = Stopwatch.StartNew();
        try
        {
            var conn = HealthCheck();
            await using var connection = new MySqlConnection(conn);
            result = await transaction.Connection.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, spName);
            throw;
        }

        LogContext.PushProperty("elapsed", sw.Elapsed.TotalMilliseconds);
        _logger.LogDebug($"{spName} {sw.Elapsed:c}");
        return result;
    }

    public async Task<T> QueryFirstAsync<T>(string name, object parameters = null)
    {
        var sw = Stopwatch.StartNew();
        T result;
        try
        {
            var conn = HealthCheck();
            await using var connection = new MySqlConnection(conn);
            result = await connection.QueryFirstAsync<T>(name, parameters, commandType: CommandType.StoredProcedure);
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, name);
            throw;
        }

        LogContext.PushProperty("elapsed", sw.Elapsed.TotalMilliseconds);
        _logger.LogDebug($"{name} {sw.Elapsed:c}");
        return result;
    }

    public int Execute(string name, IDbTransaction transaction, object parameters = null)
    {
        var sw = Stopwatch.StartNew();
        int result;
        try
        {
            result = transaction.Connection.Execute(name, parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception e)
        {
            _logger.LogError(e, name);
            throw;
        }

        LogContext.PushProperty("elapsed", sw.Elapsed.TotalMilliseconds);
        _logger.LogDebug($"{name} {sw.Elapsed:c}");
        return result;
    }

    public async Task<int> ExecuteAsync(string name, IDbTransaction transaction, object parameters = null)
    {
        var sw = Stopwatch.StartNew();
        int result;
        try
        {
            result = await transaction.Connection.ExecuteAsync(name, parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception e)
        {
            _logger.LogError(e, name);
            throw;
        }

        LogContext.PushProperty("elapsed", sw.Elapsed.TotalMilliseconds);
        _logger.LogInformation($"{name} {sw.Elapsed:c}");
        return result;
    }

    public async Task<int> ExecuteAsync(string name, DynamicParameters parameters)
    {
        int result;
        var sw = Stopwatch.StartNew();
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            result = await connection.ExecuteAsync(name, parameters, commandType: CommandType.StoredProcedure);
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, name);
            throw;
        }

        LogContext.PushProperty("elapsed", sw.Elapsed.TotalMilliseconds);
        _logger.LogDebug($"{name} {sw.Elapsed:c}");
        return result;
    }

    private string ReplaceDBString(string conn)
    {
        return conn.Substring(0, 28);
    }
}