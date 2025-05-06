namespace TalentoPro.Application.Models
{
    public class EmpleadoResultadoModel
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string IdEmpleado { get; set; }
        public object Data { get; set; }

        public static EmpleadoResultadoModel Correcto(string mensaje, string idEmpleado = null, object data = null)
        {
            return new EmpleadoResultadoModel
            {
                Exito = true,
                Mensaje = mensaje,
                IdEmpleado = idEmpleado,
                Data = data
            };
        }

        public static EmpleadoResultadoModel Error(string mensaje)
        {
            return new EmpleadoResultadoModel
            {
                Exito = false,
                Mensaje = mensaje
            };
        }
    }
} 