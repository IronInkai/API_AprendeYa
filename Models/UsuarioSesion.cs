namespace API_AprendeYa.Models
{
    public class UsuarioSesion
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public string NombreCompleto { get; set; }
        public int IdRol { get; set; }
        public string Token { get; set; }
    }
}
