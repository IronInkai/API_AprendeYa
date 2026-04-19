using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface IUsuarioService
    {
        // Método para registrar la Persona y el Usuario en una sola transacción
        Task<bool> RegistrarUsuarioAsync(RegistroRequest request);

        // Método para validar credenciales y devolver los datos del usuario junto con el Token JWT
        Task<UsuarioSesion> LoginAsync(LoginRequest request);
    }
}