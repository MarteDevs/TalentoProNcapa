using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentoPro.Domain.Entities
{
    public class Departamento
    {
        public int IdDepartamento { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public char Estado { get; private set; }

        // Propiedades de navegación
        public ICollection<Empleado> Empleados { get; private set; }

        // Constructor para nuevos departamentos
        public Departamento(string nombre, string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Estado = 'A'; // Activo por defecto
            Empleados = new List<Empleado>();
        }

        // Constructor para mapeo desde base de datos
        private Departamento()
        {
            Empleados = new List<Empleado>();
        }

        // Métodos de negocio
        public void Activar()
        {
            Estado = 'A';
        }

        public void Desactivar()
        {
            Estado = 'I';
        }

        // Métodos para actualizar el departamento
        public void ActualizarInformacion(string nombre, string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
        }
    }
}
