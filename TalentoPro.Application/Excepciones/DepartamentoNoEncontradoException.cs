using System;

namespace TalentoPro.Application.Excepciones
{
    public class DepartamentoNoEncontradoException : ApplicationException
    {
        public int IdDepartamento { get; }

        public DepartamentoNoEncontradoException(int idDepartamento) 
            : base($"No se encontró el departamento con ID: {idDepartamento}")
        {
            IdDepartamento = idDepartamento;
        }
    }
} 