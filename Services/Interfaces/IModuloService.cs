using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface IModuloService
    {
        List<Modulo> GetModulos();
        List<Modulo> GetModulosByCurso(int idCurso);
        Modulo GetModuloById(int idModulo);
        bool InsertModulo(Modulo modulo);
        bool UpdateModulo(Modulo modulo);
        bool DeleteModulo(int idModulo);
    }
}
