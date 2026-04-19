using System.ComponentModel.DataAnnotations;

namespace API_AprendeYa.Models
{
    public class Contenido
    {
        public int IdContenido { get; set; }

        public int IdTema { get; set; }

        [Required]
        public string Tipo { get; set; } 

        [Url]
        public string Url { get; set; }

        public string Texto { get; set; }

        public int Duracion { get; set; }

        public int Orden { get; set; }
    }
}
