using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TalentoPro.Domain.Entities;
using TalentoPro.Domain.Exceptions;
using TalentoPro.Domain.Repositories;
using TalentoPro.Infrastructure.Data;

namespace TalentoPro.Infrastructure.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly DBContext _dbContext;

        public EmpleadoRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Empleado> ObtenerPorId(string idEmpleado)
        {
            const string sql = @"
                SELECT e.*, d.nombre as departamento_nombre, d.descripcion as departamento_descripcion, d.estado as departamento_estado
                FROM tb_empleado e
                INNER JOIN tb_departamento d ON e.id_departamento = d.id_departamento
                WHERE e.id_empleado = @idEmpleado";

            return await _dbContext.QuerySingleAsync(sql, MapEmpleadoFromReader, new { idEmpleado });
        }

        public async Task<IEnumerable<Empleado>> ObtenerPorDepartamento(int idDepartamento)
        {
            const string sql = @"
                SELECT e.*, d.nombre as departamento_nombre, d.descripcion as departamento_descripcion, d.estado as departamento_estado
                FROM tb_empleado e
                INNER JOIN tb_departamento d ON e.id_departamento = d.id_departamento
                WHERE e.id_departamento = @idDepartamento";

            return await _dbContext.QueryAsync(sql, MapEmpleadoFromReader, new { idDepartamento });
        }

        public async Task<IEnumerable<Empleado>> Listar()
        {
            const string sql = @"
                SELECT e.*, d.nombre as departamento_nombre, d.descripcion as departamento_descripcion, d.estado as departamento_estado
                FROM tb_empleado e
                INNER JOIN tb_departamento d ON e.id_departamento = d.id_departamento
                ORDER BY e.apellido, e.nombre";

            return await _dbContext.QueryAsync(sql, MapEmpleadoFromReader);
        }

        public async Task<bool> Registrar(Empleado empleado)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@id_empleado", empleado.IdEmpleado },
                    { "@nombre", empleado.Nombre },
                    { "@apellido", empleado.Apellido },
                    { "@fecha_nacimiento", empleado.FechaNacimiento },
                    { "@fecha_contratacion", empleado.FechaContratacion },
                    { "@id_departamento", empleado.IdDepartamento },
                    { "@cargo", empleado.Cargo },
                    { "@salario", empleado.Salario },
                    { "@tipo_contrato", empleado.TipoContrato },
                    { "@nivel_educacion", empleado.NivelEducacion },
                    { "@email_corporativo", empleado.EmailCorporativo },
                    { "@estado", empleado.Estado }
                };

                var result = await _dbContext.ExecuteStoredProcedureAsync("sp_InsertarEmpleado", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Error al registrar empleado", ex);
            }
        }

        public async Task<bool> Actualizar(Empleado empleado)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@id_empleado", empleado.IdEmpleado },
                    { "@nombre", empleado.Nombre },
                    { "@apellido", empleado.Apellido },
                    { "@fecha_nacimiento", empleado.FechaNacimiento },
                    { "@fecha_contratacion", empleado.FechaContratacion },
                    { "@id_departamento", empleado.IdDepartamento },
                    { "@cargo", empleado.Cargo },
                    { "@salario", empleado.Salario },
                    { "@tipo_contrato", empleado.TipoContrato },
                    { "@nivel_educacion", empleado.NivelEducacion },
                    { "@email_corporativo", empleado.EmailCorporativo },
                    { "@estado", empleado.Estado }
                };

                var result = await _dbContext.ExecuteStoredProcedureAsync("sp_ActualizarEmpleado", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Error al actualizar empleado", ex);
            }
        }

        public async Task<bool> CambiarEstado(string idEmpleado, char estado)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@id_empleado", idEmpleado },
                    { "@estado", estado }
                };

                var result = await _dbContext.ExecuteStoredProcedureAsync("sp_CambiarEstadoEmpleado", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Error al cambiar estado del empleado", ex);
            }
        }

        public async Task<bool> ExisteEmpleadoConEmail(string email, string idEmpleadoExcluido = null)
        {
            string sql;
            object parameters;

            if (idEmpleadoExcluido != null)
            {
                sql = "SELECT COUNT(1) FROM tb_empleado WHERE email_corporativo = @email AND id_empleado != @idEmpleadoExcluido";
                parameters = new { email, idEmpleadoExcluido };
            }
            else
            {
                sql = "SELECT COUNT(1) FROM tb_empleado WHERE email_corporativo = @email";
                parameters = new { email };
            }

            var count = await _dbContext.ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }

        private Empleado MapEmpleadoFromReader(IDataReader reader)
        {
            if (reader == null) return null;

            // Mapeo de la entidad Departamento
            var departamento = new Departamento(
                reader["departamento_nombre"].ToString(),
                reader["departamento_descripcion"].ToString()
            );

            // Utilizamos el constructor privado Empleado() para crear la instancia
            // y luego configuramos sus propiedades mediante reflexión
            var empleado = new Empleado(
                reader["id_empleado"].ToString(),
                reader["nombre"].ToString(),
                reader["apellido"].ToString(),
                Convert.ToDateTime(reader["fecha_nacimiento"]),
                Convert.ToDateTime(reader["fecha_contratacion"]),
                Convert.ToInt32(reader["id_departamento"]),
                reader["cargo"].ToString(),
                Convert.ToDecimal(reader["salario"]),
                reader["tipo_contrato"].ToString(),
                reader["nivel_educacion"] == DBNull.Value ? null : reader["nivel_educacion"].ToString(),
                reader["email_corporativo"].ToString()
            );

            // Configurar el estado desde la base de datos
            if (reader["estado"] != DBNull.Value)
            {
                var estado = reader["estado"].ToString()[0];
                if (estado != 'A') // Si no es activo, actualizamos el estado
                {
                    empleado.CambiarEstado(estado);
                }
            }

            // Para acceder a la propiedad de navegación Departamento, necesitaríamos 
            // una forma de establecerla, pero como es privada, en un entorno real 
            // podríamos usar reflexión o un método interno para configurarla

            return empleado;
        }
    }
} 