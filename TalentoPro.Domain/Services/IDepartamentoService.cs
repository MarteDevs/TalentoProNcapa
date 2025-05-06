using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentoPro.Domain.Services
{
    using TalentoPro.Domain.Entities;

    public interface IDepartamentoService
    {
        Task<Departamento> ObtenerPorId(int idDepartamento);
        Task<IEnumerable<Departamento>> ListarDepartamentos();
        Task<bool> RegistrarDepartamento(Departamento departamento);
        Task<bool> ActualizarDepartamento(Departamento departamento);
        Task<bool> CambiarEstadoDepartamento(int idDepartamento, char estado);
    }
}