using Microsoft.AspNetCore.Mvc;
using SistemaBancario.API.Interfaces;
using SistemaBancario.API.Repositories;
using System.Linq;

namespace SistemaBancario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly CuentaRepository _cuentaRepository;

        public DashboardController(IClienteRepository clienteRepository, CuentaRepository cuentaRepository)
        {
            _clienteRepository = clienteRepository;
            _cuentaRepository = cuentaRepository;
        }

        // GET: api/dashboard/1
        [HttpGet("{clienteId}")]
        public IActionResult ObtenerResumenCliente(int clienteId)
        {
            var cliente = _clienteRepository.ObtenerPorId(clienteId);
            if (cliente == null) return NotFound(new { mensaje = "Cliente no encontrado." });

            var cuentas = _cuentaRepository.ObtenerCuentasPorCliente(clienteId);

            // Armamos un objeto anónimo (DTO al vuelo) para enviar como JSON
            var resumen = new
            {
                nombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}",
                documentoIdentidad = cliente.DocumentoIdentidad,
                patrimonioTotal = cuentas.Sum(c => c.Saldo),
                cuentas = cuentas
            };

            return Ok(resumen);
        }
    }
}