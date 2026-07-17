using Microsoft.Data.SqlClient;
using SistemaBancario.API.Models;
using System;
using System.Data;

namespace SistemaBancario.API.Repositories
{
    public class CuentaRepository
    {
        private readonly string _connectionString;

        public CuentaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método de Transferencia Segura (Grado Bancario)
        public bool TransferirDineroSeguro(int idCuentaOrigen, int idCuentaDestino, decimal monto)
        {
            if (monto <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // INICIA LA TRANSACCIÓN: A partir de aquí, o se ejecuta TODO o no se ejecuta NADA
                using (SqlTransaction transaccion = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Verificar saldo suficiente (Bloqueando la fila temporalmente para evitar doble gasto)
                        string queryCheck = "SELECT Saldo FROM Cuentas WITH (UPDLOCK) WHERE Id = @IdOrigen AND Activo = 1";
                        decimal saldoDisponible = 0;

                        using (SqlCommand cmdCheck = new SqlCommand(queryCheck, conn, transaccion))
                        {
                            cmdCheck.Parameters.AddWithValue("@IdOrigen", idCuentaOrigen);
                            object result = cmdCheck.ExecuteScalar();

                            if (result == null) throw new Exception("Cuenta de origen no encontrada o inactiva.");
                            saldoDisponible = Convert.ToDecimal(result);

                            if (saldoDisponible < monto) throw new Exception("Fondos insuficientes.");
                        }

                        // 2. Descontar de la cuenta origen (Débito)
                        string queryDebito = "UPDATE Cuentas SET Saldo = Saldo - @Monto WHERE Id = @IdOrigen";
                        using (SqlCommand cmdDebito = new SqlCommand(queryDebito, conn, transaccion))
                        {
                            cmdDebito.Parameters.AddWithValue("@Monto", monto);
                            cmdDebito.Parameters.AddWithValue("@IdOrigen", idCuentaOrigen);
                            cmdDebito.ExecuteNonQuery();
                        }

                        // 3. Aumentar en la cuenta destino (Crédito)
                        string queryCredito = "UPDATE Cuentas SET Saldo = Saldo + @Monto WHERE Id = @IdDestino";
                        using (SqlCommand cmdCredito = new SqlCommand(queryCredito, conn, transaccion))
                        {
                            cmdCredito.Parameters.AddWithValue("@Monto", monto);
                            cmdCredito.Parameters.AddWithValue("@IdDestino", idCuentaDestino);
                            cmdCredito.ExecuteNonQuery();
                        }

                        // 4. Registrar en la auditoría (Historial)
                        string queryAuditoria = @"INSERT INTO HistorialTransferencias (CuentaOrigenId, CuentaDestinoId, Monto, Estado) 
                                                  VALUES (@IdOrigen, @IdDestino, @Monto, 'COMPLETADO')";
                        using (SqlCommand cmdAuditoria = new SqlCommand(queryAuditoria, conn, transaccion))
                        {
                            cmdAuditoria.Parameters.AddWithValue("@IdOrigen", idCuentaOrigen);
                            cmdAuditoria.Parameters.AddWithValue("@IdDestino", idCuentaDestino);
                            cmdAuditoria.Parameters.AddWithValue("@Monto", monto);
                            cmdAuditoria.ExecuteNonQuery();
                        }

                        // SI TODO SALIÓ PERFECTO, GUARDAMOS LOS CAMBIOS DEFINITIVAMENTE
                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // SI OCURRIÓ CUALQUIER ERROR (Falta de saldo, error de red, base de datos caída), REVERTIMOS TODO
                        transaccion.Rollback();
                        // En un entorno real, aquí se registra el error (log)
                        return false;
                    }
                }
            }
        }
        public IEnumerable<Cuenta> ObtenerCuentasActivas()
        {
            var cuentas = new List<Cuenta>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Solo traemos las columnas estrictamente necesarias para la lista desplegable
                string query = "SELECT Id, NumeroCuenta FROM Cuentas WHERE Activo = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cuentas.Add(new Cuenta
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                NumeroCuenta = reader["NumeroCuenta"].ToString()
                            });
                        }
                    }
                }
            }
            return cuentas;
        }

        public IEnumerable<Cuenta> ObtenerCuentasPorCliente(int clienteId)
        {
            var cuentas = new List<Cuenta>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, NumeroCuenta, TipoCuenta, Saldo FROM Cuentas WHERE ClienteId = @ClienteId AND Activo = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", clienteId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cuentas.Add(new Cuenta
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                NumeroCuenta = reader["NumeroCuenta"].ToString(),
                                TipoCuenta = reader["TipoCuenta"].ToString(),
                                Saldo = Convert.ToDecimal(reader["Saldo"])
                            });
                        }
                    }
                }
            }
            return cuentas;
        }

        public bool RetirarDinero(int cuentaId, decimal monto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Actualizamos restando el monto SOLO si el saldo actual es mayor o igual a lo que se quiere retirar
                string query = "UPDATE Cuentas SET Saldo = Saldo - @Monto WHERE Id = @Id AND Saldo >= @Monto AND Activo = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", cuentaId);
                    cmd.Parameters.AddWithValue("@Monto", monto);

                    conn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Si filasAfectadas es 1, el retiro fue un éxito. Si es 0, no tenía fondos o la cuenta no existe.
                    return filasAfectadas > 0;
                }
            }
        }
    }
}
