using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface INivelService
    {
        List<Nivel> GetNiveles();
        Nivel GetNivelById(int idNivel);
        bool InsertNivel(Nivel nivel);
        bool UpdateNivel(Nivel nivel);
        bool DeleteNivel(int idNivel);
    }
}
