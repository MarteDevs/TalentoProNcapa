using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TalentoPro.Domain.Exceptions;
using Dapper;

namespace TalentoPro.Infrastructure.Data
{
    public class DBContext
    {
        private readonly ConexionBD _conexionBD;

        public DBContext(ConexionBD conexionBD)
        {
            _conexionBD = conexionBD ?? throw new ArgumentNullException(nameof(conexionBD));
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null)
        {
            using (var connection = _conexionBD.CreateConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    var result = await SqlMapper.ExecuteScalarAsync<T>(connection, sql, parameters);
                    return result;
                }
                catch (SqlException ex)
                {
                    throw new InfrastructureException("Error al ejecutar consulta escalar en la base de datos", ex);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, Func<IDataReader, T> mapper, object parameters = null)
        {
            using (var connection = _conexionBD.CreateConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        AddParameters(command, parameters);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var results = new List<T>();
                            while (reader.Read())
                            {
                                results.Add(mapper(reader));
                            }
                            return results;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new InfrastructureException("Error al ejecutar consulta en la base de datos", ex);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        public async Task<T> QuerySingleAsync<T>(string sql, Func<IDataReader, T> mapper, object parameters = null)
        {
            using (var connection = _conexionBD.CreateConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        AddParameters(command, parameters);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                return mapper(reader);
                            }
                            return default;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new InfrastructureException("Error al ejecutar consulta single en la base de datos", ex);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            using (var connection = _conexionBD.CreateConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    return await connection.ExecuteAsync(sql, parameters);
                }
                catch (SqlException ex)
                {
                    throw new InfrastructureException("Error al ejecutar comando en la base de datos", ex);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        public async Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, Dictionary<string, object> parameters = null)
        {
            using (var connection = _conexionBD.CreateConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    DynamicParameters dynamicParameters = null;
                    
                    if (parameters != null)
                    {
                        dynamicParameters = new DynamicParameters();
                        foreach (var param in parameters)
                        {
                            dynamicParameters.Add(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    
                    return await connection.ExecuteAsync(
                        storedProcedureName, 
                        dynamicParameters, 
                        commandType: CommandType.StoredProcedure);
                }
                catch (SqlException ex)
                {
                    throw new InfrastructureException($"Error al ejecutar el procedimiento almacenado {storedProcedureName}", ex);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        private void AddParameters(SqlCommand command, object parameters)
        {
            if (parameters != null)
            {
                var props = parameters.GetType().GetProperties();
                foreach (var prop in props)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"@{prop.Name}";
                    parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }
        }
    }
} 