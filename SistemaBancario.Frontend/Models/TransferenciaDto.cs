using System.ComponentModel.DataAnnotations;

namespace SistemaBancario.Frontend.Models
{
    public class TransferenciaDto
    {
        [Required(ErrorMessage = "Seleccione la cuenta de origen.")]
        public int IdCuentaOrigen { get; set; }

        [Required(ErrorMessage = "Seleccione la cuenta de destino.")]
        public int IdCuentaDestino { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.1, 100000, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }
    }
}