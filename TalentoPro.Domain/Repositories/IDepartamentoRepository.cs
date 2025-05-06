using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentoPro.Domain.Repositories
{
    using TalentoPro.Domain.Entities;

    public interface IDepartamentoRepository
    {
        Task<Departamento> ObtenerPorId(int idDepartamento);
        Task<IEnumerable<Departamento>> Listar();
        Task<bool> Registrar(Departamento departamento);
        Task<bool> Actualizar(Departamento departamento);
        Task<bool> CambiarEstado(int idDepartamento, char estado);
    }
}