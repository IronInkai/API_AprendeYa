using System.ComponentModel.DataAnnotations;

namespace API_AprendeYa.Models
{
    public class Nivel
    {

        public int IdNivel { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

    }
}
