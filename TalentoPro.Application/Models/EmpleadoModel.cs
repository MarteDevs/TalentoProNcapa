using System;

namespace TalentoPro.Application.Models
{
    public class EmpleadoModel
    {
        public string IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaContratacion { get; set; }
        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; }
        public string Cargo { get; set; }
        public decimal Salario { get; set; }
        public string TipoContrato { get; set; }
        public string NivelEducacion { get; set; }
        public string EmailCorporativo { get; set; }
        public char Estado { get; set; }
        public int Antiguedad { get; set; }
        public decimal Beneficios { get; set; }
    }
} 