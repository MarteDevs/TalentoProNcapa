using System;

namespace TalentoPro.Application.Excepciones
{
    public class EmpleadoNoEncontradoException : ApplicationException
    {
        public string IdEmpleado { get; }

        public EmpleadoNoEncontradoException(string idEmpleado) 
            : base($"No se encontró el empleado con ID: {idEmpleado}")
        {
            IdEmpleado = idEmpleado;
        }
    }
} 