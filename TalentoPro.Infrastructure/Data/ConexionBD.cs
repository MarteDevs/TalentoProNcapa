using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace TalentoPro.Infrastructure.Data
{
    public class ConexionBD
    {
        private readonly string _connectionString;

        public ConexionBD(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RecursosHumanosDB") 
                ?? throw new ArgumentNullException("La cadena de conexión 'RecursosHumanosDB' no está configurada.");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
} 