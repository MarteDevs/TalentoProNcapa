using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentoPro.Domain.Services
{
    using TalentoPro.Domain.Entities;
    using TalentoPro.Domain.Exceptions;
    using TalentoPro.Domain.Repositories;

    public class ServicioEmpleado : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;

        public ServicioEmpleado(
            IEmpleadoRepository empleadoRepository,
            IDepartamentoRepository departamentoRepository)
        {
            _empleadoRepository = empleadoRepository ?? throw new ArgumentNullException(nameof(empleadoRepository));
            _departamentoRepository = departamentoRepository ?? throw new ArgumentNullException(nameof(departamentoRepository));
        }

        public async Task<Empleado> ObtenerPorId(string idEmpleado)
        {
            return await _empleadoRepository.ObtenerPorId(idEmpleado);
        }

        public async Task<IEnumerable<Empleado>> ObtenerPorDepartamento(int idDepartamento)
        {
            var departamento = await _departamentoRepository.ObtenerPorId(idDepartamento);
            if (departamento == null)
                throw new DomainException($"El departamento con ID {idDepartamento} no existe");

            return await _empleadoRepository.ObtenerPorDepartamento(idDepartamento);
        }

        public async Task<IEnumerable<Empleado>> ListarEmpleados()
        {
            return await _empleadoRepository.Listar();
        }

        public async Task<bool> RegistrarEmpleado(Empleado empleado)
        {
            // Validar que el email sea único
            if (await _empleadoRepository.ExisteEmpleadoConEmail(empleado.EmailCorporativo))
                throw new DomainException("Ya existe un empleado con el mismo email corporativo");

            // Validar que el departamento exista
            var departamento = await _departamentoRepository.ObtenerPorId(empleado.IdDepartamento);
            if (departamento == null)
                throw new DomainException($"El departamento con ID {empleado.IdDepartamento} no existe");

            // Registrar empleado
            return await _empleadoRepository.Registrar(empleado);
        }

        public async Task<bool> ActualizarEmpleado(Empleado empleado)
        {
            // Validar que el email sea único (excluyendo al propio empleado)
            if (await _empleadoRepository.ExisteEmpleadoConEmail(empleado.EmailCorporativo, empleado.IdEmpleado))
                throw new DomainException("Ya existe otro empleado con el mismo email corporativo");

            // Validar que el departamento exista
            var departamento = await _departamentoRepository.ObtenerPorId(empleado.IdDepartamento);
            if (departamento == null)
                throw new DomainException($"El departamento con ID {empleado.IdDepartamento} no existe");

            // Actualizar empleado
            return await _empleadoRepository.Actualizar(empleado);
        }

        public async Task<bool> CambiarEstadoEmpleado(string idEmpleado, char estado)
        {
            var empleado = await _empleadoRepository.ObtenerPorId(idEmpleado);
            if (empleado == null)
                throw new DomainException($"El empleado con ID {idEmpleado} no existe");

            return await _empleadoRepository.CambiarEstado(idEmpleado, estado);
        }

        public async Task<decimal> CalcularSalarioRecomendado(string cargo, string nivelEducacion)
        {
            // Base salarial según cargo
            decimal salarioBase = cargo switch
            {
                "Desarrollador Junior" => 2500.00m,
                "Desarrollador Senior" => 4500.00m,
                "Arquitecto de Software" => 5000.00m,
                "Especialista en Selección" => 3200.00m,
                "Contador" => 3800.00m,
                "Diseñador Gráfico" => 3000.00m,
                _ => 2000.00m // Salario base por defecto
            };

            // Ajuste por nivel de educación
            decimal factorEducacion = nivelEducacion switch
            {
                "Doctorado" => 1.4m,
                "Maestría" => 1.3m,
                "Licenciatura" => 1.2m,
                "Técnico" => 1.1m,
                _ => 1.0m // Sin ajuste por defecto
            };

            return Math.Round(salarioBase * factorEducacion, 2);
        }

        public async Task<decimal> CalcularBeneficiosAdicionales(string idEmpleado)
        {
            var empleado = await _empleadoRepository.ObtenerPorId(idEmpleado);
            if (empleado == null)
                throw new DomainException($"El empleado con ID {idEmpleado} no existe");

            // Beneficios base según entidad
            decimal beneficiosBase = empleado.CalcularBeneficios();

            // Beneficios adicionales según reglas de negocio específicas
            int antiguedad = empleado.CalcularAntiguedad();

            // Bonus por cada 5 años de servicio
            decimal bonusAntiguedad = (antiguedad / 5) * (empleado.Salario * 0.05m);

            // Bonus por nivel educativo
            decimal bonusEducacion = empleado.NivelEducacion switch
            {
                "Doctorado" => empleado.Salario * 0.10m,
                "Maestría" => empleado.Salario * 0.07m,
                "Licenciatura" => empleado.Salario * 0.05m,
                _ => 0m
            };

            return beneficiosBase + bonusAntiguedad + bonusEducacion;
        }

        public async Task<bool> ValidarUnicidadEmail(string email, string idEmpleadoExcluido = null)
        {
            return !await _empleadoRepository.ExisteEmpleadoConEmail(email, idEmpleadoExcluido);
        }
    }
}