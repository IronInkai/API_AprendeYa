using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface ICarritoService
    {
        RespuestaCarrito AgregarAlCarrito(int idUsuario, int idCurso);

        CarritoCompras ObtenerCarrito(int idUsuario);

        RespuestaCarrito EliminarDelCarrito(int idUsuario, int idDetalle);

        RespuestaCarrito PagarCarrito(int idUsuario);
    }
}