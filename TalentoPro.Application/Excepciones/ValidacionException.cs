using System;
using System.Collections.Generic;

namespace TalentoPro.Application.Excepciones
{
    public class ValidacionException : ApplicationException
    {
        public List<string> Errores { get; }

        public ValidacionException(List<string> errores) 
            : base("Error de validación: " + string.Join(", ", errores))
        {
            Errores = errores;
        }
    }
} 