using System.Text.Json.Serialization;

namespace API_AprendeYa.Models
{
    public class RespuestaForo
    {
        public int IdRespuesta { get; set; }
        public int IdTema { get; set; }
        public int IdUsuario { get; set; }

        public string Contenido { get; set; }
        public DateTime Fecha { get; set; }

        public string ImagenUrl { get; set; }
        public bool EsMejorRespuesta { get; set; }


        [JsonIgnore]
        public Usuario Usuario { get; set; }

        [JsonIgnore]
        public TemaForo Tema { get; set; }
    }
}
