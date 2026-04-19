using System.ComponentModel.DataAnnotations;

namespace API_AprendeYa.Models
{
    public class Curso
    {
        public int idCurso { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Range(0, 9999)]
        public decimal Precio { get; set; }

        public int IdNivel { get; set; }

        public int IdInstructor { get; set; }

        public int IdCategoria { get; set; }

        [Url]
        public string ImagenUrl { get; set; }

        public string Estado { get; set; }
    }
}
