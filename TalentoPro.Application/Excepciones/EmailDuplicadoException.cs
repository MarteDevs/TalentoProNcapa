using System;

namespace TalentoPro.Application.Excepciones
{
    public class EmailDuplicadoException : ApplicationException
    {
        public string Email { get; }

        public EmailDuplicadoException(string email) 
            : base($"Ya existe un empleado con el email corporativo: {email}")
        {
            Email = email;
        }
    }
} 