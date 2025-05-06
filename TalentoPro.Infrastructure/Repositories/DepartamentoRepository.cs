using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TalentoPro.Domain.Entities;
using TalentoPro.Domain.Exceptions;
using TalentoPro.Domain.Repositories;
using TalentoPro.Infrastructure.Data;

namespace TalentoPro.Infrastructure.Repositories
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly DBContext _dbContext;

        public DepartamentoRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Departamento> ObtenerPorId(int idDepartamento)
        {
            const string sql = @"
                SELECT id_departamento, nombre, descripcion, estado
                FROM tb_departamento
                WHERE id_departamento = @idDepartamento";

            return await _dbContext.QuerySingleAsync(sql, MapDepartamentoFromReader, new { idDepartamento });
        }

        public async Task<IEnumerable<Departamento>> Listar()
        {
            try
            {
                var result = await _dbContext.ExecuteStoredProcedureAsync("sp_ListarDepartamentos");
                return await _dbContext.QueryAsync("SELECT * FROM tb_departamento WHERE estado = 'A' ORDER BY nombre", MapDepartamentoFromReader);
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Error al listar departamentos", ex);
            }
        }

        public async Task<bool> Registrar(Departamento departamento)
        {
            try
            {
                const string sql = @"
                    INSERT INTO tb_departamento (nombre, descripcion, estado)
                    VALUES (@nombre, @descripcion, @estado);
                    SELECT SCOPE_IDENTITY();";

                var parameters = new
                {
                    nombre = departamento.Nombre,
                    descripcion = departamento.Descripcion,
                    estado = departamento.Estado
                };

                var result = await _dbContext.ExecuteScalarAsync<int>(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Error al registrar departamento", ex);
            }
        }

        public async Task<bool> Actualizar(Departamento departamento)
        {
            try
            {
                const string sql = @"
                    UPDATE tb_departamento
                    SET nombre = @nombre, 
                        descripcion = @descripcion, 
                        estado = @estado
                    WHERE id_departamento = @idDepartamento";

                var parameters = new
                {
                    idDepartamento = departamento.IdDepartamento,
                    nombre = departamento.Nombre,
                    descripcion = departamento.Descripcion,
                    estado = departamento.Estado
                };

                var result = await _dbContext.ExecuteAsync(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Error al actualizar departamento", ex);
            }
        }

        public async Task<bool> CambiarEstado(int idDepartamento, char estado)
        {
            try
            {
                const string sql = @"
                    UPDATE tb_departamento
                    SET estado = @estado
                    WHERE id_departamento = @idDepartamento";

                var parameters = new
                {
                    idDepartamento,
                    estado
                };

                var result = await _dbContext.ExecuteAsync(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Error al cambiar estado del departamento", ex);
            }
        }

        private Departamento MapDepartamentoFromReader(IDataReader reader)
        {
            if (reader == null) return null;

            var departamento = new Departamento(
                reader["nombre"].ToString(),
                reader["descripcion"].ToString()
            );

            // En un entorno real, usaríamos reflexión o un método interno para establecer la propiedad IdDepartamento
            // ya que tiene un setter privado, pero aquí lo simulamos

            // Si el estado es diferente de 'A', actualizamos el estado
            if (reader["estado"] != DBNull.Value && reader["estado"].ToString()[0] != 'A')
            {
                departamento.Desactivar();
            }

            return departamento;
        }
    }
} 