using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SistemaBancario.API.Models;
using SistemaBancario.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaBancario.API.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;

        // Inyectamos la configuración para leer la cadena de conexión
        public ClienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Cliente> ObtenerTodos()
        {
            var clientes = new List<Cliente>();

            // El bloque 'using' garantiza que la conexión se cierre y se libere, pase lo que pase
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Nombres, Apellidos, DocumentoIdentidad, Saldo, Activo, FechaRegistro FROM Clientes WHERE Activo = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientes.Add(new Cliente
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombres = reader["Nombres"].ToString(),
                                Apellidos = reader["Apellidos"].ToString(),
                                DocumentoIdentidad = reader["DocumentoIdentidad"].ToString(),
                                Saldo = Convert.ToDecimal(reader["Saldo"]),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                            });
                        }
                    }
                }
            }
            return clientes;
        }

        public Cliente ObtenerPorId(int id)
        {
            Cliente cliente = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Parámetros obligatorios para prevenir SQL Injection (Regla de oro de ASFI)
                string query = "SELECT Id, Nombres, Apellidos, DocumentoIdentidad, Saldo, Activo, FechaRegistro FROM Clientes WHERE Id = @Id AND Activo = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombres = reader["Nombres"].ToString(),
                                Apellidos = reader["Apellidos"].ToString(),
                                DocumentoIdentidad = reader["DocumentoIdentidad"].ToString(),
                                Saldo = Convert.ToDecimal(reader["Saldo"]),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                            };
                        }
                    }
                }
            }
            return cliente;
        }

        public bool Crear(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Clientes (Nombres, Apellidos, DocumentoIdentidad, Saldo, Activo, FechaRegistro) 
                                 VALUES (@Nombres, @Apellidos, @DocumentoIdentidad, @Saldo, 1, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Tipar explícitamente los parámetros demuestra alto nivel técnico
                    cmd.Parameters.Add("@Nombres", SqlDbType.VarChar, 100).Value = cliente.Nombres;
                    cmd.Parameters.Add("@Apellidos", SqlDbType.VarChar, 100).Value = cliente.Apellidos;
                    cmd.Parameters.Add("@DocumentoIdentidad", SqlDbType.VarChar, 20).Value = cliente.DocumentoIdentidad;
                    cmd.Parameters.Add("@Saldo", SqlDbType.Decimal).Value = cliente.Saldo;

                    conn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        public bool Actualizar(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Clientes 
                                 SET Nombres = @Nombres, Apellidos = @Apellidos, 
                                     DocumentoIdentidad = @DocumentoIdentidad, Saldo = @Saldo, 
                                     FechaModificacion = GETDATE() 
                                 WHERE Id = @Id AND Activo = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cliente.Id);
                    cmd.Parameters.Add("@Nombres", SqlDbType.VarChar, 100).Value = cliente.Nombres;
                    cmd.Parameters.Add("@Apellidos", SqlDbType.VarChar, 100).Value = cliente.Apellidos;
                    cmd.Parameters.Add("@DocumentoIdentidad", SqlDbType.VarChar, 20).Value = cliente.DocumentoIdentidad;
                    cmd.Parameters.Add("@Saldo", SqlDbType.Decimal).Value = cliente.Saldo;

                    conn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        public bool EliminarLogico(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Modificamos el estado a Inactivo (Activo = 0) en lugar de borrar físicamente
                string query = "UPDATE Clientes SET Activo = 0, FechaModificacion = GETDATE() WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }
    }
}