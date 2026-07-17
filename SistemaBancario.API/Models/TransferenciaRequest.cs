namespace SistemaBancario.API.Models
{
    public class TransferenciaRequest
    {
        public int IdCuentaOrigen { get; set; }
        public int IdCuentaDestino { get; set; }
        public decimal Monto { get; set; }
    }
}