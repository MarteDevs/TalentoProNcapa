using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentoPro.Domain.Exceptions;

namespace TalentoPro.Domain.Entities
{
    public class Empleado
    {
        public string IdEmpleado { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public DateTime FechaNacimiento { get; private set; }
        public DateTime FechaContratacion { get; private set; }
        public int IdDepartamento { get; private set; }
        public string Cargo { get; private set; }
        public decimal Salario { get; private set; }
        public string TipoContrato { get; private set; }
        public string NivelEducacion { get; private set; }
        public string EmailCorporativo { get; private set; }
        public char Estado { get; private set; }

        // Propiedades de navegación
        public Departamento Departamento { get; private set; }

        // Constructor para nuevos empleados
        public Empleado(
            string idEmpleado,
            string nombre,
            string apellido,
            DateTime fechaNacimiento,
            DateTime fechaContratacion,
            int idDepartamento,
            string cargo,
            decimal salario,
            string tipoContrato,
            string nivelEducacion,
            string emailCorporativo)
        {
            IdEmpleado = idEmpleado;
            Nombre = nombre;
            Apellido = apellido;
            FechaNacimiento = fechaNacimiento;
            FechaContratacion = fechaContratacion;
            IdDepartamento = idDepartamento;
            Cargo = cargo;
            Salario = salario;
            TipoContrato = tipoContrato;
            NivelEducacion = nivelEducacion;
            EmailCorporativo = emailCorporativo;
            Estado = 'A'; // Activo por defecto

            ValidarEmailCorporativo();
        }

        // Constructor para mapeo desde base de datos
        private Empleado() { }

        // Métodos de negocio
        public int CalcularAntiguedad()
        {
            return DateTime.Today.Year - FechaContratacion.Year -
                (DateTime.Today.DayOfYear < FechaContratacion.DayOfYear ? 1 : 0);
        }

        public decimal CalcularBeneficios()
        {
            decimal beneficioBase = 0;
            int antiguedad = CalcularAntiguedad();

            // Beneficio base según tipo de contrato
            switch (TipoContrato)
            {
                case "Tiempo completo":
                    beneficioBase = Salario * 0.15m;
                    break;
                case "Medio tiempo":
                    beneficioBase = Salario * 0.08m;
                    break;
                case "Por proyecto":
                    beneficioBase = Salario * 0.05m;
                    break;
                default:
                    beneficioBase = 0;
                    break;
            }

            // Beneficio adicional por antigüedad
            if (antiguedad >= 5)
                beneficioBase += Salario * 0.05m * (antiguedad / 5);

            return beneficioBase;
        }

        public bool ValidarEmailCorporativo()
        {
            if (!EmailCorporativo.EndsWith("@empresa.com"))
                throw new DomainException("El email corporativo debe terminar con @empresa.com");

            return true;
        }

        // Métodos para actualizar el empleado
        public void ActualizarInformacionPersonal(
            string nombre,
            string apellido,
            DateTime fechaNacimiento)
        {
            Nombre = nombre;
            Apellido = apellido;
            FechaNacimiento = fechaNacimiento;
        }

        public void ActualizarInformacionLaboral(
            DateTime fechaContratacion,
            int idDepartamento,
            string cargo,
            decimal salario,
            string tipoContrato,
            string nivelEducacion)
        {
            FechaContratacion = fechaContratacion;
            IdDepartamento = idDepartamento;
            Cargo = cargo;
            Salario = salario;
            TipoContrato = tipoContrato;
            NivelEducacion = nivelEducacion;
        }

        public void ActualizarEmailCorporativo(string emailCorporativo)
        {
            EmailCorporativo = emailCorporativo;
            ValidarEmailCorporativo();
        }

        public void CambiarEstado(char estado)
        {
            if (estado != 'A' && estado != 'I' && estado != 'C')
                throw new DomainException("Estado de empleado no válido. Use A (Activo), I (Inactivo) o C (Cesado)");

            Estado = estado;
        }
    }
}