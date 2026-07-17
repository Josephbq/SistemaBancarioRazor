using System.ComponentModel.DataAnnotations;

namespace SistemaBancario.Frontend.Models
{
    public class RetiroDto
    {
        [Required(ErrorMessage = "Seleccione la cuenta desde donde desea retirar.")]
        public int CuentaId { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(1, 10000, ErrorMessage = "El monto debe estar entre 1 y 10,000 Bs.")]
        public decimal Monto { get; set; }
    }
}