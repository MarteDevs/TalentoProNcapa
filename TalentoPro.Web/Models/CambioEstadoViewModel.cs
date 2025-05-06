using System.ComponentModel.DataAnnotations;

namespace TalentoPro.Web.Models
{
    public class CambioEstadoViewModel
    {
        public string IdEmpleado { get; set; }
        
        public string Nombre { get; set; }
        
        public string Apellido { get; set; }
        
        [Display(Name = "Estado Actual")]
        public char Estado { get; set; }
        
        [Required(ErrorMessage = "El nuevo estado es obligatorio")]
        [Display(Name = "Nuevo Estado")]
        public char NuevoEstado { get; set; }
        
        public string EstadoTexto => Estado switch
        {
            'A' => "Activo",
            'I' => "Inactivo",
            'C' => "Cesado",
            _ => "Desconocido"
        };
    }
} 