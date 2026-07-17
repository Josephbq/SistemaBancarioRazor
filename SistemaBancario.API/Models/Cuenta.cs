namespace SistemaBancario.API.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public decimal Saldo { get; set; }
    }
}
