using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPro.Application.Models;

namespace TalentoPro.Application.Servicios
{
    public interface IEmpleadoServicio
    {
        Task<EmpleadoModel> ObtenerEmpleadoPorId(string idEmpleado);
        Task<IEnumerable<EmpleadoModel>> ObtenerEmpleadosPorDepartamento(int idDepartamento);
        Task<IEnumerable<EmpleadoModel>> ListarEmpleados();
        Task<EmpleadoResultadoModel> RegistrarEmpleado(EmpleadoSolicitudModel empleadoModel);
        Task<EmpleadoResultadoModel> ActualizarEmpleado(EmpleadoSolicitudModel empleadoModel);
        Task<EmpleadoResultadoModel> CambiarEstadoEmpleado(string idEmpleado, char estado);
        Task<decimal> CalcularSalarioRecomendado(string cargo, string nivelEducacion);
        Task<decimal> CalcularBeneficiosEmpleado(string idEmpleado);
    }
} 