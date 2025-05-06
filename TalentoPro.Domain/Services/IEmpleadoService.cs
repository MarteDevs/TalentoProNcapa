using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentoPro.Domain.Services
{
    using TalentoPro.Domain.Entities;

    public interface IEmpleadoService
    {
        Task<Empleado> ObtenerPorId(string idEmpleado);
        Task<IEnumerable<Empleado>> ObtenerPorDepartamento(int idDepartamento);
        Task<IEnumerable<Empleado>> ListarEmpleados();
        Task<bool> RegistrarEmpleado(Empleado empleado);
        Task<bool> ActualizarEmpleado(Empleado empleado);
        Task<bool> CambiarEstadoEmpleado(string idEmpleado, char estado);
        Task<decimal> CalcularSalarioRecomendado(string cargo, string nivelEducacion);
        Task<decimal> CalcularBeneficiosAdicionales(string idEmpleado);
        Task<bool> ValidarUnicidadEmail(string email, string idEmpleadoExcluido = null);
    }
}