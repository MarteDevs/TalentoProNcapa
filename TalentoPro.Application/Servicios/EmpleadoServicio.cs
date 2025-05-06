using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentoPro.Application.Excepciones;
using TalentoPro.Application.Models;
using TalentoPro.Application.Validaciones.Empleado;
using TalentoPro.Domain.Entities;
using TalentoPro.Domain.Services;

namespace TalentoPro.Application.Servicios
{
    public class EmpleadoServicio : IEmpleadoServicio
    {
        private readonly IEmpleadoService _empleadoService;
        private readonly ValidadorEmpleado _validadorEmpleado;

        public EmpleadoServicio(IEmpleadoService empleadoService, ValidadorEmpleado validadorEmpleado)
        {
            _empleadoService = empleadoService ?? throw new ArgumentNullException(nameof(empleadoService));
            _validadorEmpleado = validadorEmpleado ?? throw new ArgumentNullException(nameof(validadorEmpleado));
        }

        public async Task<EmpleadoModel> ObtenerEmpleadoPorId(string idEmpleado)
        {
            try
            {
                var empleado = await _empleadoService.ObtenerPorId(idEmpleado);
                if (empleado == null)
                {
                    throw new EmpleadoNoEncontradoException(idEmpleado);
                }

                return MapearAEmpleadoModel(empleado);
            }
            catch (Exception ex)
            {
                throw ManejadorExcepciones.TraducirExcepcion(ex);
            }
        }

        public async Task<IEnumerable<EmpleadoModel>> ObtenerEmpleadosPorDepartamento(int idDepartamento)
        {
            try
            {
                var empleados = await _empleadoService.ObtenerPorDepartamento(idDepartamento);
                return empleados.Select(MapearAEmpleadoModel);
            }
            catch (Exception ex)
            {
                throw ManejadorExcepciones.TraducirExcepcion(ex);
            }
        }

        public async Task<IEnumerable<EmpleadoModel>> ListarEmpleados()
        {
            try
            {
                var empleados = await _empleadoService.ListarEmpleados();
                return empleados.Select(MapearAEmpleadoModel);
            }
            catch (Exception ex)
            {
                throw ManejadorExcepciones.TraducirExcepcion(ex);
            }
        }

        public async Task<EmpleadoResultadoModel> RegistrarEmpleado(EmpleadoSolicitudModel empleadoModel)
        {
            try
            {
                // Validar empleado
                var resultadoValidacion = await _validadorEmpleado.ValidarEntidad(empleadoModel);
                if (!resultadoValidacion.EsValido)
                {
                    throw new ValidacionException(resultadoValidacion.Errores);
                }

                // Verificar si el email ya existe
                var emailUnico = await _empleadoService.ValidarUnicidadEmail(empleadoModel.EmailCorporativo);
                if (!emailUnico)
                {
                    throw new EmailDuplicadoException(empleadoModel.EmailCorporativo);
                }

                // Mapear a entidad de dominio
                var empleado = MapearAEmpleadoEntidad(empleadoModel);

                // Registrar empleado
                var resultado = await _empleadoService.RegistrarEmpleado(empleado);

                if (resultado)
                {
                    return EmpleadoResultadoModel.Correcto(
                        "Empleado registrado correctamente", 
                        empleadoModel.IdEmpleado);
                }
                else
                {
                    return EmpleadoResultadoModel.Error("No se pudo registrar el empleado");
                }
            }
            catch (ValidacionException ex)
            {
                return EmpleadoResultadoModel.Error(ex.Message);
            }
            catch (Exception ex)
            {
                var excepcionTraducida = ManejadorExcepciones.TraducirExcepcion(ex);
                return EmpleadoResultadoModel.Error(excepcionTraducida.Message);
            }
        }

        public async Task<EmpleadoResultadoModel> ActualizarEmpleado(EmpleadoSolicitudModel empleadoModel)
        {
            try
            {
                // Verificar que el empleado exista
                var empleadoExistente = await _empleadoService.ObtenerPorId(empleadoModel.IdEmpleado);
                if (empleadoExistente == null)
                {
                    throw new EmpleadoNoEncontradoException(empleadoModel.IdEmpleado);
                }

                // Validar empleado
                var resultadoValidacion = await _validadorEmpleado.ValidarEntidad(empleadoModel);
                if (!resultadoValidacion.EsValido)
                {
                    throw new ValidacionException(resultadoValidacion.Errores);
                }

                // Verificar unicidad de email (excluyendo el empleado actual)
                var emailUnico = await _empleadoService.ValidarUnicidadEmail(
                    empleadoModel.EmailCorporativo, empleadoModel.IdEmpleado);
                
                if (!emailUnico)
                {
                    throw new EmailDuplicadoException(empleadoModel.EmailCorporativo);
                }

                // Mapear a entidad de dominio
                var empleado = MapearAEmpleadoEntidad(empleadoModel);

                // Actualizar empleado
                var resultado = await _empleadoService.ActualizarEmpleado(empleado);

                if (resultado)
                {
                    return EmpleadoResultadoModel.Correcto(
                        "Empleado actualizado correctamente", 
                        empleadoModel.IdEmpleado);
                }
                else
                {
                    return EmpleadoResultadoModel.Error("No se pudo actualizar el empleado");
                }
            }
            catch (ValidacionException ex)
            {
                return EmpleadoResultadoModel.Error(ex.Message);
            }
            catch (Exception ex)
            {
                var excepcionTraducida = ManejadorExcepciones.TraducirExcepcion(ex);
                return EmpleadoResultadoModel.Error(excepcionTraducida.Message);
            }
        }

        public async Task<EmpleadoResultadoModel> CambiarEstadoEmpleado(string idEmpleado, char estado)
        {
            try
            {
                // Verificar que el estado sea válido
                if (estado != 'A' && estado != 'I' && estado != 'C')
                {
                    throw new TalentoPro.Application.Excepciones.ApplicationException("Estado no válido. Debe ser A (Activo), I (Inactivo) o C (Cesado)");
                }

                // Verificar que el empleado exista
                var empleadoExistente = await _empleadoService.ObtenerPorId(idEmpleado);
                if (empleadoExistente == null)
                {
                    throw new EmpleadoNoEncontradoException(idEmpleado);
                }

                // Cambiar estado
                var resultado = await _empleadoService.CambiarEstadoEmpleado(idEmpleado, estado);

                if (resultado)
                {
                    string estadoTexto = estado switch
                    {
                        'A' => "Activo",
                        'I' => "Inactivo",
                        'C' => "Cesado",
                        _ => "Desconocido"
                    };

                    return EmpleadoResultadoModel.Correcto(
                        $"Estado del empleado cambiado a: {estadoTexto}", 
                        idEmpleado);
                }
                else
                {
                    return EmpleadoResultadoModel.Error("No se pudo cambiar el estado del empleado");
                }
            }
            catch (Exception ex)
            {
                var excepcionTraducida = ManejadorExcepciones.TraducirExcepcion(ex);
                return EmpleadoResultadoModel.Error(excepcionTraducida.Message);
            }
        }

        public async Task<decimal> CalcularSalarioRecomendado(string cargo, string nivelEducacion)
        {
            try
            {
                return await _empleadoService.CalcularSalarioRecomendado(cargo, nivelEducacion);
            }
            catch (Exception ex)
            {
                throw ManejadorExcepciones.TraducirExcepcion(ex);
            }
        }

        public async Task<decimal> CalcularBeneficiosEmpleado(string idEmpleado)
        {
            try
            {
                // Verificar que el empleado exista
                var empleadoExistente = await _empleadoService.ObtenerPorId(idEmpleado);
                if (empleadoExistente == null)
                {
                    throw new EmpleadoNoEncontradoException(idEmpleado);
                }

                return await _empleadoService.CalcularBeneficiosAdicionales(idEmpleado);
            }
            catch (Exception ex)
            {
                throw ManejadorExcepciones.TraducirExcepcion(ex);
            }
        }

        // Métodos de mapeo
        private EmpleadoModel MapearAEmpleadoModel(Empleado empleado)
        {
            return new EmpleadoModel
            {
                IdEmpleado = empleado.IdEmpleado,
                Nombre = empleado.Nombre,
                Apellido = empleado.Apellido,
                FechaNacimiento = empleado.FechaNacimiento,
                FechaContratacion = empleado.FechaContratacion,
                IdDepartamento = empleado.IdDepartamento,
                NombreDepartamento = empleado.Departamento?.Nombre,
                Cargo = empleado.Cargo,
                Salario = empleado.Salario,
                TipoContrato = empleado.TipoContrato,
                NivelEducacion = empleado.NivelEducacion,
                EmailCorporativo = empleado.EmailCorporativo,
                Estado = empleado.Estado,
                Antiguedad = empleado.CalcularAntiguedad(),
                Beneficios = empleado.CalcularBeneficios()
            };
        }

        private Empleado MapearAEmpleadoEntidad(EmpleadoSolicitudModel model)
        {
            return new Empleado(
                model.IdEmpleado,
                model.Nombre,
                model.Apellido,
                model.FechaNacimiento,
                model.FechaContratacion,
                model.IdDepartamento,
                model.Cargo,
                model.Salario,
                model.TipoContrato,
                model.NivelEducacion,
                model.EmailCorporativo);
        }
    }
} 