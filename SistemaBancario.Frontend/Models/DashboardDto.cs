using System.Collections.Generic;

namespace SistemaBancario.Frontend.Models
{
    public class CuentaDto
    {
        public int Id { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public decimal Saldo { get; set; }
    }

    public class DashboardDto
    {
        public string NombreCompleto { get; set; }
        public string DocumentoIdentidad { get; set; }
        public decimal PatrimonioTotal { get; set; }
        public List<CuentaDto> Cuentas { get; set; } = new List<CuentaDto>();
    }
}