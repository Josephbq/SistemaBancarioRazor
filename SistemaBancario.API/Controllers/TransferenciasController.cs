using Microsoft.AspNetCore.Mvc;
using SistemaBancario.API.Models;
using SistemaBancario.API.Repositories;
using System;

namespace SistemaBancario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferenciasController : ControllerBase
    {
        private readonly CuentaRepository _cuentaRepository;

        public TransferenciasController(CuentaRepository cuentaRepository)
        {
            _cuentaRepository = cuentaRepository;
        }

        // GET: api/transferencias/cuentas/1
        // Para llenar los dropdowns en el frontend
        [HttpGet("cuentas/{clienteId}")]
        public IActionResult ObtenerCuentasDisponibles(int clienteId)
        {
            var cuentas = _cuentaRepository.ObtenerCuentasPorCliente(clienteId);
            return Ok(cuentas);
        }

        // POST: api/transferencias
        [HttpPost]
        public IActionResult ProcesarTransferencia([FromBody] TransferenciaRequest request)
        {
            if (request.IdCuentaOrigen == request.IdCuentaDestino)
            {
                return BadRequest(new { mensaje = "La cuenta de origen y destino no pueden ser la misma." });
            }

            if (request.Monto <= 0)
            {
                return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });
            }

            try
            {
                // Invocamos nuestra transacción ACID nivel bancario
                bool exito = _cuentaRepository.TransferirDineroSeguro(request.IdCuentaOrigen, request.IdCuentaDestino, request.Monto);

                if (exito)
                {
                    return Ok(new { mensaje = $"Transferencia de {request.Monto} Bs. completada exitosamente." });
                }

                return BadRequest(new { mensaje = "Transacción rechazada: Verifique su saldo disponible." });
            }
            catch (Exception)
            {
                // En un banco real, aquí guardamos el log de errores.
                return StatusCode(500, new { mensaje = "Error interno del servidor al procesar la transacción." });
            }
        }

        // Creamos una clase rápida aquí mismo para recibir el JSON del retiro
        public class RetiroRequest
        {
            public int CuentaId { get; set; }
            public decimal Monto { get; set; }
        }

        // POST: api/transferencias/retiro
        [HttpPost("retiro")]
        public IActionResult ProcesarRetiro([FromBody] RetiroRequest request)
        {
            if (request.Monto <= 0)
            {
                return BadRequest(new { mensaje = "El monto a retirar debe ser mayor a cero." });
            }

            try
            {
                bool exito = _cuentaRepository.RetirarDinero(request.CuentaId, request.Monto);

                if (exito)
                {
                    return Ok(new { mensaje = $"Retiro de {request.Monto} Bs. procesado con éxito." });
                }

                return BadRequest(new { mensaje = "Operación denegada: Fondos insuficientes." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error interno al procesar el retiro." });
            }
        }
    }
}