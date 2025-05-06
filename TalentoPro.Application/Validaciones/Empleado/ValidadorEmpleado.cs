using System.Threading.Tasks;
using TalentoPro.Application.Models;

namespace TalentoPro.Application.Validaciones.Empleado
{
    public class ValidadorEmpleado : ValidadorBase<EmpleadoSolicitudModel>
    {
        public ValidadorEmpleado()
        {
            // Agregamos todas las validaciones que queremos aplicar
            AgregarValidacion(new ValidacionEmpleadoNombreApellido());
            AgregarValidacion(new ValidacionEmpleadoEmail());
            AgregarValidacion(new ValidacionEmpleadoFechas());
            AgregarValidacion(new ValidacionEmpleadoSalario());
            // Nuevas validaciones pueden agregarse aquí sin modificar esta clase
        }
    }
} 