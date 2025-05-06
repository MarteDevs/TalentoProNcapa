using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentoPro.Application.Validaciones
{
    public abstract class ValidadorBase<T>
    {
        private readonly List<IValidacion<T>> _validaciones = new List<IValidacion<T>>();

        protected void AgregarValidacion(IValidacion<T> validacion)
        {
            _validaciones.Add(validacion);
        }

        public async Task<ValidationResult> ValidarEntidad(T entidad)
        {
            var errores = new List<string>();

            foreach (var validacion in _validaciones)
            {
                var resultadosValidacion = await validacion.Validar(entidad);
                if (resultadosValidacion != null && resultadosValidacion.Any())
                {
                    errores.AddRange(resultadosValidacion);
                }
            }

            return new ValidationResult(errores);
        }
    }

    public class ValidationResult
    {
        public List<string> Errores { get; }
        public bool EsValido => !Errores.Any();

        public ValidationResult(List<string> errores)
        {
            Errores = errores ?? new List<string>();
        }
    }
} 