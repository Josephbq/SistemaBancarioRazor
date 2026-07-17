namespace SistemaBancario.Frontend.Models
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string DocumentoIdentidad { get; set; }
        public decimal Saldo { get; set; }
        public bool Activo { get; set; }
    }
}