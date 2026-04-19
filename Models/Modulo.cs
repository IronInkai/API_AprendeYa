using System.ComponentModel.DataAnnotations;

namespace API_AprendeYa.Models
{
    public class Modulo
    {
        public int IdModulo { get; set; }

        public int IdCurso { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public int Orden { get; set; }
    }
}
