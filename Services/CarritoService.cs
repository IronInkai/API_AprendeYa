using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;

namespace API_AprendeYa.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly string _connectionString;

        public CarritoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public RespuestaCarrito AgregarAlCarrito(int idUsuario, int idCurso)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Obtener precio del curso a agregar
                        decimal precioCurso = connection.QuerySingleOrDefault<decimal>(
                            "SELECT precio FROM curso WHERE id_curso = @Id",
                            new { Id = idCurso }, transaction);

                        // 2. Validar si el alumno ya COMPRÓ este curso antes
                        int yaComprado = connection.ExecuteScalar<int>(@"
                            SELECT COUNT(1) FROM venta v 
                            INNER JOIN venta_curso vc ON v.id_venta = vc.id_venta 
                            WHERE v.id_usuario = @IdUsuario AND vc.id_curso = @IdCurso AND v.estado = 'Completado'",
                            new { IdUsuario = idUsuario, IdCurso = idCurso }, transaction);

                        if (yaComprado > 0) return new RespuestaCarrito { Exito = false, Mensaje = "Ya posees este curso en tu biblioteca." };

                        // 3. Buscar si el usuario tiene un carrito Activo, si no, lo creamos
                        int? idCarrito = connection.QuerySingleOrDefault<int?>(
                            "SELECT id_carrito FROM carrito WHERE id_usuario = @IdUsuario AND estado = 'Activo'",
                            new { IdUsuario = idUsuario }, transaction);

                        if (idCarrito == null || idCarrito == 0)
                        {
                            idCarrito = connection.QuerySingle<int>(
                                "INSERT INTO carrito (id_usuario, estado) OUTPUT INSERTED.id_carrito VALUES (@IdUsuario, 'Activo')",
                                new { IdUsuario = idUsuario }, transaction);
                        }

                        // 4. Validar si el curso YA ESTÁ en el carrito_detalle
                        int yaEnCarrito = connection.ExecuteScalar<int>(
                            "SELECT COUNT(1) FROM carrito_detalle WHERE id_carrito = @IdCarrito AND id_curso = @IdCurso",
                            new { IdCarrito = idCarrito, IdCurso = idCurso }, transaction);

                        if (yaEnCarrito > 0) return new RespuestaCarrito { Exito = false, Mensaje = "Este curso ya está en tu carrito." };

                        // 5. Insertar el curso en el carrito_detalle
                        connection.Execute(
                            "INSERT INTO carrito_detalle (id_carrito, id_curso, precio) VALUES (@IdCarrito, @IdCurso, @Precio)",
                            new { IdCarrito = idCarrito, IdCurso = idCurso, Precio = precioCurso }, transaction);

                        // 6. Actualizar o crear la "Cartera" (El resumen de subtotales)
                        int? idCartera = connection.QuerySingleOrDefault<int?>(
                            "SELECT id_cartera FROM cartera WHERE id_carrito = @IdCarrito",
                            new { IdCarrito = idCarrito }, transaction);

                        if (idCartera == null || idCartera == 0)
                        {
                            connection.Execute(
                                "INSERT INTO cartera (id_carrito, subtotal, descuento, total) VALUES (@IdCarrito, @Precio, 0, @Precio)",
                                new { IdCarrito = idCarrito, Precio = precioCurso }, transaction);
                        }
                        else
                        {
                            connection.Execute(
                                "UPDATE cartera SET subtotal = subtotal + @Precio, total = total + @Precio WHERE id_cartera = @IdCartera",
                                new { Precio = precioCurso, IdCartera = idCartera }, transaction);
                        }

                        transaction.Commit();
                        return new RespuestaCarrito { Exito = true, Mensaje = "¡Curso agregado al carrito con éxito!" };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new RespuestaCarrito { Exito = false, Mensaje = "Error interno: " + ex.Message };
                    }
                }
            }
        }
        public CarritoCompras ObtenerCarrito(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // 1. Buscamos el carrito activo y su total en la cartera
                // 1. Buscamos el carrito activo y su total en la cartera
                string sqlCart = @"
                    SELECT c.id_carrito AS IdCarrito, ISNULL(ca.total, 0) AS Total
                    FROM carrito c
                    LEFT JOIN cartera ca ON c.id_carrito = ca.id_carrito
                    WHERE c.id_usuario = @IdUsuario AND c.estado = 'Activo'";

                // CAMBIO AQUÍ: Usamos QueryFirstOrDefault en lugar de QuerySingleOrDefault
                var carrito = connection.QueryFirstOrDefault<CarritoCompras>(sqlCart, new { IdUsuario = idUsuario });

                // Si no tiene carrito activo, devolvemos un carrito vacío
                if (carrito == null) return new CarritoCompras();

                // 2. Buscamos los cursos dentro de ese carrito
                string sqlItems = @"
                    SELECT 
                        cd.id_detalle AS IdDetalle, 
                        c.id_curso AS IdCurso, 
                        c.titulo AS Titulo, 
                        c.imagen_url AS ImagenUrl, 
                        cd.precio AS Precio
                    FROM carrito_detalle cd
                    INNER JOIN curso c ON cd.id_curso = c.id_curso
                    WHERE cd.id_carrito = @IdCarrito";

                carrito.Items = connection.Query<ItemCarrito>(sqlItems, new { IdCarrito = carrito.IdCarrito }).ToList();

                return carrito;
            }
        }
        public RespuestaCarrito EliminarDelCarrito(int idUsuario, int idDetalle)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Buscamos el ítem para saber cuánto costaba y a qué carrito pertenece
                        string sqlItem = @"
                            SELECT cd.precio, c.id_carrito 
                            FROM carrito_detalle cd
                            INNER JOIN carrito c ON cd.id_carrito = c.id_carrito
                            WHERE cd.id_detalle = @IdDetalle AND c.id_usuario = @IdUsuario AND c.estado = 'Activo'";

                        var item = connection.QueryFirstOrDefault(sqlItem, new { IdDetalle = idDetalle, IdUsuario = idUsuario }, transaction);

                        if (item == null) return new RespuestaCarrito { Exito = false, Mensaje = "No se encontró el curso en tu carrito." };

                        decimal precio = item.precio;
                        int idCarrito = item.id_carrito;

                        // 2. Eliminamos el curso del detalle
                        connection.Execute("DELETE FROM carrito_detalle WHERE id_detalle = @IdDetalle",
                            new { IdDetalle = idDetalle }, transaction);

                        // 3. Le restamos el precio a la cartera
                        connection.Execute(@"
                            UPDATE cartera 
                            SET subtotal = subtotal - @Precio, total = total - @Precio 
                            WHERE id_carrito = @IdCarrito",
                            new { Precio = precio, IdCarrito = idCarrito }, transaction);

                        transaction.Commit();
                        return new RespuestaCarrito { Exito = true, Mensaje = "Curso eliminado del carrito." };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new RespuestaCarrito { Exito = false, Mensaje = "Error al eliminar: " + ex.Message };
                    }
                }
            }
        }
        public RespuestaCarrito PagarCarrito(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Buscamos el carrito activo y su total
                        string sqlCart = @"
                            SELECT c.id_carrito, ISNULL(ca.total, 0) as total 
                            FROM carrito c 
                            LEFT JOIN cartera ca ON c.id_carrito = ca.id_carrito 
                            WHERE c.id_usuario = @IdUsuario AND c.estado = 'Activo'";

                        var cart = connection.QueryFirstOrDefault(sqlCart, new { IdUsuario = idUsuario }, transaction);

                        if (cart == null || cart.total <= 0)
                            return new RespuestaCarrito { Exito = false, Mensaje = "Tu carrito está vacío o no es válido para pagar." };

                        int idCarrito = (int)cart.id_carrito;
                        decimal total = (decimal)cart.total;

                        // 2. Creamos la Venta Oficial y capturamos su nuevo ID
                        string sqlVenta = @"
                            INSERT INTO venta (id_usuario, fecha, total, estado) 
                            OUTPUT INSERTED.id_venta 
                            VALUES (@IdUsuario, GETDATE(), @Total, 'Completado')";

                        int idVenta = connection.QuerySingle<int>(sqlVenta, new { IdUsuario = idUsuario, Total = total }, transaction);

                        // 3. Copiamos todos los cursos del carrito hacia venta_curso
                        string sqlVentaCurso = @"
                            INSERT INTO venta_curso (id_venta, id_curso, precio) 
                            SELECT @IdVenta, id_curso, precio 
                            FROM carrito_detalle 
                            WHERE id_carrito = @IdCarrito";

                        connection.Execute(sqlVentaCurso, new { IdVenta = idVenta, IdCarrito = idCarrito }, transaction);

                        // 4. Marcamos el carrito como Pagado (para que ya no le salga al usuario)
                        connection.Execute("UPDATE carrito SET estado = 'Pagado' WHERE id_carrito = @IdCarrito",
                            new { IdCarrito = idCarrito }, transaction);

                        transaction.Commit();
                        return new RespuestaCarrito { Exito = true, Mensaje = "¡Pago procesado con éxito! Tus cursos están listos." };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new RespuestaCarrito { Exito = false, Mensaje = "Error al procesar el pago: " + ex.Message };
                    }
                }
            }
        }
    }
}