namespace API_AprendeYa.Models
{
    public class VotoRespuesta
    {
        public int IdVoto { get; set; }
        public int IdRespuesta { get; set; }
        public int IdUsuario { get; set; }

        public bool Tipo { get; set; } 

        public RespuestaForo Respuesta { get; set; }

    }
}
