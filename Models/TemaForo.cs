namespace API_AprendeYa.Models
{
    public class TemaForo
    {
        public int IdTema { get; set; }
        public int IdForo { get; set; }
        public int IdUsuario { get; set; }

        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; }

        public string ImagenUrl { get; set; }
        public int Vistas { get; set; }

        public Usuario Usuario { get; set; }

        public List<RespuestaForo> Respuestas { get; set; }
    }
}