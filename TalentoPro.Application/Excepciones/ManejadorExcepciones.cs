using System;
using TalentoPro.Domain.Exceptions;

namespace TalentoPro.Application.Excepciones
{
    public static class ManejadorExcepciones
    {
        public static Exception TraducirExcepcion(Exception ex)
        {
            return ex switch
            {
                DomainException domainException when domainException.Message.Contains("departamento no existe") =>
                    new DepartamentoNoEncontradoException(ExtractIdFromMessage(domainException.Message)),

                DomainException domainException when domainException.Message.Contains("empleado no existe") =>
                    new EmpleadoNoEncontradoException(ExtractStringIdFromMessage(domainException.Message)),

                DomainException domainException when domainException.Message.Contains("email corporativo") =>
                    new EmailDuplicadoException(ExtractEmailFromMessage(domainException.Message)),

                InfrastructureException infrastructureException =>
                    new ApplicationException("Error en la capa de infraestructura: " + infrastructureException.Message, infrastructureException),

                _ => new ApplicationException("Error inesperado: " + ex.Message, ex)
            };
        }

        private static int ExtractIdFromMessage(string message)
        {
            var parts = message.Split(':');
            if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int id))
            {
                return id;
            }
            return 0;
        }

        private static string ExtractStringIdFromMessage(string message)
        {
            var parts = message.Split(':');
            if (parts.Length > 1)
            {
                return parts[1].Trim();
            }
            return string.Empty;
        }

        private static string ExtractEmailFromMessage(string message)
        {
            var parts = message.Split(':');
            if (parts.Length > 1)
            {
                return parts[1].Trim();
            }
            return string.Empty;
        }
    }
} 