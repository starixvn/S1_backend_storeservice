using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Serilog.Context;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Library;

public class MySqlClient
{
    public DataBaseEnum DataBase { get; set; }

    private readonly string _connectionString;

    private readonly ILogger<MySqlClient> _logger;

    public MySqlClient(string connectionString, ILogger<MySqlClient> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public MySqlConnection GetMySqlConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    #region Query

    public T QueryFirst<T>(string name, object parameters)
    {
        T result;
        var sw = Stopwatch.StartNew();
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var command = new CommandDefinition(name, parameters, 
                commandType: CommandType.StoredProcedure,
                flags: CommandFlags.NoCache);
            result = connection.QueryFirstOrDefault<T>(command);
            connection.Close();
            connection.Dispose();
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
    
    public IEnumerable<T> Query<T>(string spName, object parameters = null, CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null)
    {
        IEnumerable<T> result;
        var sw = Stopwatch.StartNew();
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            result = connection.Query<T>(spName, parameters, commandTimeout: commandTimeout, commandType: commandType);
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
    
    public async Task<IEnumerable<T>> QueryAsync<T>(string spName, object parameters = null, CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null)
    {
        IEnumerable<T> result;
        var sw = Stopwatch.StartNew();
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            result = await connection.QueryAsync<T>(spName, parameters, commandTimeout: commandTimeout, commandType: commandType);
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

    public async Task<IEnumerable<T>> QueryAsync<T>(string spName,  IDbTransaction transaction , object parameters = null)
    {
        IEnumerable<T> result;
        var sw = Stopwatch.StartNew();
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
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

    public async Task<T> QueryFirstAsync<T>(string name, IDbTransaction transaction, object parameters = null)
    {
        var sw = Stopwatch.StartNew();
        T result;
        try
        {
            result = await transaction.Connection.QueryFirstAsync<T>(name, parameters, transaction,
                commandType: CommandType.StoredProcedure);
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
    
    public async Task<T> QueryFirstAsync<T>(string name, object parameters = null)
    {
        var sw = Stopwatch.StartNew();
        T result;
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
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
    
    #endregion

    #region Execute

    public int Execute(string name, CommandType commandType = CommandType.StoredProcedure, object parameters = null)
    {
        var sw = Stopwatch.StartNew();
        int result;
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            result = connection.Execute(name, parameters, commandType: commandType);
            connection.Close();
            connection.Dispose();
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

    #endregion
   
}