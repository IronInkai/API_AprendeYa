using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface IContenidoService
    {
        List<Contenido> GetContenidos();
        Contenido GetContenidoById(int idContenido);
        bool InsertContenido(Contenido contenido);
        bool UpdateContenido(Contenido contenido);
        bool DeleteContenido(int idContenido);
    }
}

