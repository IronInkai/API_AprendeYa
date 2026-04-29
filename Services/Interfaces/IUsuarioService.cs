using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface IUsuarioService
    {
        // === MÉTODOS DE AUTENTICACIÓN ===
        Task<bool> RegistrarUsuarioAsync(RegistroRequest request);
        Task<UsuarioSesion> LoginAsync(LoginRequest request);

        // === MÉTODOS CRUD ADMINISTRADOR ===
        List<UsuarioAdmin> GetUsuarios();
        List<CursoMatriculado> ObtenerCursosDeAlumno(int idUsuario);
        UsuarioAdmin GetUsuarioById(int idUsuario);
        bool InsertUsuario(UsuarioAdmin usuario);
        bool UpdateUsuario(UsuarioAdmin usuario);
        bool DeleteUsuario(int idUsuario);
        bool MatricularAlumno(int idUsuario, int idCurso);
    }
}