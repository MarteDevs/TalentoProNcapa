using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPro.Application.Models;

namespace TalentoPro.Application.Validaciones.Empleado
{
    public class ValidacionEmpleadoEmail : IValidacion<EmpleadoSolicitudModel>
    {
        public Task<List<string>> Validar(EmpleadoSolicitudModel entidad)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(entidad.EmailCorporativo))
            {
                errores.Add("El email corporativo es obligatorio.");
            }
            else if (!entidad.EmailCorporativo.EndsWith("@empresa.com"))
            {
                errores.Add("El email corporativo debe terminar con @empresa.com");
            }
            else if (entidad.EmailCorporativo.Length < 10 || entidad.EmailCorporativo.Length > 150)
            {
                errores.Add("El email corporativo debe tener entre 10 y 150 caracteres.");
            }

            return Task.FromResult(errores);
        }
    }
} 