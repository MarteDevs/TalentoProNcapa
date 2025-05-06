using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentoPro.Domain.Repositories
{
    using TalentoPro.Domain.Entities;
    
    public interface IEmpleadoRepository
    {
        Task<Empleado> ObtenerPorId(string idEmpleado);
        Task<IEnumerable<Empleado>> ObtenerPorDepartamento(int idDepartamento);
        Task<IEnumerable<Empleado>> Listar();
        Task<bool> Registrar(Empleado empleado);
        Task<bool> Actualizar(Empleado empleado);
        Task<bool> CambiarEstado(string idEmpleado, char estado);
        Task<bool> ExisteEmpleadoConEmail(string email, string idEmpleadoExcluido = null);
    }
}