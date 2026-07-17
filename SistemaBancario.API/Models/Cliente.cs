using System.ComponentModel.DataAnnotations;

namespace SistemaBancario.API.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El documento de identidad es obligatorio")]
        [Display(Name = "C.I. / Documento")]
        public string DocumentoIdentidad { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El saldo no puede ser negativo")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Saldo { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
