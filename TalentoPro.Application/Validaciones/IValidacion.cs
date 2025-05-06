using System.Collections.Generic;
using System.Threading.Tasks;

namespace TalentoPro.Application.Validaciones
{
    public interface IValidacion<T>
    {
        Task<List<string>> Validar(T entidad);
    }
} 