using Microsoft.AspNetCore.Mvc;
using SistemaBancario.API.Interfaces;
using SistemaBancario.API.Models;
using System.Collections.Generic;

namespace SistemaBancario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        // Inyectamos el repositorio que configuraste en el paso anterior
        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // GET: api/clientes
        [HttpGet]
        public ActionResult<IEnumerable<Cliente>> ObtenerTodos()
        {
            var clientes = _clienteRepository.ObtenerTodos();

            // Devuelve un HTTP 200 (OK) con la lista convertida a JSON
            return Ok(clientes);
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public ActionResult<Cliente> ObtenerPorId(int id)
        {
            var cliente = _clienteRepository.ObtenerPorId(id);
            if (cliente == null)
            {
                return NotFound(new { mensaje = "Cliente no encontrado en el sistema." }); // HTTP 404
            }
            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public ActionResult Crear([FromBody] Cliente cliente)
        {
            if (cliente == null) return BadRequest("Datos inválidos.");

            bool creado = _clienteRepository.Crear(cliente);
            if (creado)
            {
                // HTTP 200 OK con un mensaje de confirmación en JSON
                return Ok(new { mensaje = "Cliente registrado con éxito en la ASFI." });
            }

            return StatusCode(500, new { mensaje = "Error interno al guardar en la base de datos." });
        }

        // PUT: api/clientes/baja/5
        [HttpPut("baja/{id}")]
        public ActionResult DarDeBaja(int id)
        {
            // Ejecutamos el borrado lógico que armamos en ADO.NET
            bool eliminado = _clienteRepository.EliminarLogico(id);
            if (eliminado)
            {
                return Ok(new { mensaje = "Cuenta de cliente suspendida correctamente." });
            }
            return BadRequest(new { mensaje = "No se pudo suspender la cuenta del cliente." });
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public ActionResult Actualizar(int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del cliente." });
            }

            bool actualizado = _clienteRepository.Actualizar(cliente);
            if (actualizado)
            {
                return Ok(new { mensaje = "Datos del cliente actualizados correctamente." });
            }

            return StatusCode(500, new { mensaje = "Error al actualizar los datos en el servidor." });
        }
    }
}