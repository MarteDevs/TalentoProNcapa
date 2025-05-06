using System;

namespace TalentoPro.Application.Models
{
    public class EmpleadoSolicitudModel
    {
        public string IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaContratacion { get; set; }
        public int IdDepartamento { get; set; }
        public string Cargo { get; set; }
        public decimal Salario { get; set; }
        public string TipoContrato { get; set; }
        public string NivelEducacion { get; set; }
        public string EmailCorporativo { get; set; }
    }
} 