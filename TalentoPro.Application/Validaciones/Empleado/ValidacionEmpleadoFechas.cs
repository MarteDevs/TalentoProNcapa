using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPro.Application.Models;

namespace TalentoPro.Application.Validaciones.Empleado
{
    public class ValidacionEmpleadoFechas : IValidacion<EmpleadoSolicitudModel>
    {
        public Task<List<string>> Validar(EmpleadoSolicitudModel entidad)
        {
            var errores = new List<string>();
            var hoy = DateTime.Today;

            // Validar edad (mínimo 18 años, máximo 70 años)
            var edad = hoy.Year - entidad.FechaNacimiento.Year;
            if (entidad.FechaNacimiento > hoy.AddYears(-edad))
            {
                edad--;
            }

            if (edad < 18)
            {
                errores.Add("El empleado debe ser mayor de 18 años.");
            }
            else if (edad > 70)
            {
                errores.Add("La edad del empleado no puede ser mayor a 70 años.");
            }

            // La fecha de contratación no puede ser futura
            if (entidad.FechaContratacion > hoy)
            {
                errores.Add("La fecha de contratación no puede ser futura.");
            }

            // La fecha de contratación no puede ser anterior a la fecha de nacimiento + 18 años
            var fechaMinContratacion = entidad.FechaNacimiento.AddYears(18);
            if (entidad.FechaContratacion < fechaMinContratacion)
            {
                errores.Add("La fecha de contratación no puede ser anterior a la mayoría de edad del empleado.");
            }

            return Task.FromResult(errores);
        }
    }
} 