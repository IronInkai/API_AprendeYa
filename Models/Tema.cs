using System.ComponentModel.DataAnnotations;

namespace API_AprendeYa.Models
{
    public class Tema
    {
        public int IdTema { get; set; }

        public int IdModulo { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public int Orden { get; set; }
    }
}
