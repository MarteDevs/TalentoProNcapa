using System.ComponentModel.DataAnnotations;

namespace TalentoPro.Web.Models
{
    public class BeneficiosViewModel
    {
        public string IdEmpleado { get; set; }
        
        public string Nombre { get; set; }
        
        public string Apellido { get; set; }
        
        public string Cargo { get; set; }
        
        [Display(Name = "Salario")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Salario { get; set; }
        
        [Display(Name = "Antigüedad (años)")]
        public int Antiguedad { get; set; }
        
        [Display(Name = "Beneficios Calculados")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal BeneficiosCalculados { get; set; }
        
        [Display(Name = "Total (Salario + Beneficios)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total => Salario + BeneficiosCalculados;
        
        [Display(Name = "Porcentaje sobre salario")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public decimal PorcentajeBeneficios => Salario > 0 ? BeneficiosCalculados / Salario : 0;
    }
} 