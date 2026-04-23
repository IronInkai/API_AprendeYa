namespace API_AprendeYa.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public int Puntos { get; set; }

        public ICollection<RespuestaForo> Respuestas { get; set; }
    }
}
