using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPro.Application.Models;

namespace TalentoPro.Application.Validaciones.Empleado
{
    public class ValidacionEmpleadoSalario : IValidacion<EmpleadoSolicitudModel>
    {
        public Task<List<string>> Validar(EmpleadoSolicitudModel entidad)
        {
            var errores = new List<string>();

            if (entidad.Salario <= 0)
            {
                errores.Add("El salario debe ser mayor que cero.");
            }
            else if (entidad.Salario > 50000)
            {
                errores.Add("El salario no puede exceder los 50,000.");
            }

            // Validaciones según tipo de contrato
            switch (entidad.TipoContrato)
            {
                case "Tiempo completo":
                    if (entidad.Salario < 2000)
                    {
                        errores.Add("El salario mínimo para un contrato a tiempo completo es 2,000.");
                    }
                    break;
                case "Medio tiempo":
                    if (entidad.Salario < 1000)
                    {
                        errores.Add("El salario mínimo para un contrato a medio tiempo es 1,000.");
                    }
                    break;
                case "Por proyecto":
                    // Sin validación específica para proyectos
                    break;
                default:
                    errores.Add("El tipo de contrato especificado no es válido.");
                    break;
            }

            return Task.FromResult(errores);
        }
    }
} 