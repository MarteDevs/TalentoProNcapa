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

    public class ServicioDepartamento : IDepartamentoService
    {
        private readonly IDepartamentoRepository _departamentoRepository;

        public ServicioDepartamento(IDepartamentoRepository departamentoRepository)
        {
            _departamentoRepository = departamentoRepository ?? throw new ArgumentNullException(nameof(departamentoRepository));
        }

        public async Task<Departamento> ObtenerPorId(int idDepartamento)
        {
            return await _departamentoRepository.ObtenerPorId(idDepartamento);
        }

        public async Task<IEnumerable<Departamento>> ListarDepartamentos()
        {
            return await _departamentoRepository.Listar();
        }

        public async Task<bool> RegistrarDepartamento(Departamento departamento)
        {
            return await _departamentoRepository.Registrar(departamento);
        }

        public async Task<bool> ActualizarDepartamento(Departamento departamento)
        {
            var departamentoExistente = await _departamentoRepository.ObtenerPorId(departamento.IdDepartamento);
            if (departamentoExistente == null)
                throw new DomainException($"El departamento con ID {departamento.IdDepartamento} no existe");

            return await _departamentoRepository.Actualizar(departamento);
        }

        public async Task<bool> CambiarEstadoDepartamento(int idDepartamento, char estado)
        {
            if (estado != 'A' && estado != 'I')
                throw new DomainException("Estado de departamento no válido. Use A (Activo) o I (Inactivo)");

            var departamento = await _departamentoRepository.ObtenerPorId(idDepartamento);
            if (departamento == null)
                throw new DomainException($"El departamento con ID {idDepartamento} no existe");

            return await _departamentoRepository.CambiarEstado(idDepartamento, estado);
        }
    }
}