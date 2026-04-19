using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface ITemaService
    {
        List<Tema> GetTemas();
        Tema GetTemaById(int idTema);
        bool InsertTema(Tema tema);
        bool UpdateTema(Tema tema);
        bool DeleteTema(int idTema);
    }
}
