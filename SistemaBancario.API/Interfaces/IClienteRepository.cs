using SistemaBancario.API.Models;
using System.Collections.Generic;

namespace SistemaBancario.API.Interfaces
{
    public interface IClienteRepository
    {
        IEnumerable<Cliente> ObtenerTodos();
        Cliente ObtenerPorId(int id);
        bool Crear(Cliente cliente);
        bool Actualizar(Cliente cliente);
        bool EliminarLogico(int id); // En banca no se borra, se desactiva
    }
}