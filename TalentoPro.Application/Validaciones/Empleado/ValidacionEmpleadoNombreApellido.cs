using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPro.Application.Models;

namespace TalentoPro.Application.Validaciones.Empleado
{
    public class ValidacionEmpleadoNombreApellido : IValidacion<EmpleadoSolicitudModel>
    {
        public Task<List<string>> Validar(EmpleadoSolicitudModel entidad)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(entidad.Nombre))
            {
                errores.Add("El nombre del empleado es obligatorio.");
            }
            else if (entidad.Nombre.Length < 2 || entidad.Nombre.Length > 100)
            {
                errores.Add("El nombre del empleado debe tener entre 2 y 100 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(entidad.Apellido))
            {
                errores.Add("El apellido del empleado es obligatorio.");
            }
            else if (entidad.Apellido.Length < 2 || entidad.Apellido.Length > 100)
            {
                errores.Add("El apellido del empleado debe tener entre 2 y 100 caracteres.");
            }

            return Task.FromResult(errores);
        }
    }
} 