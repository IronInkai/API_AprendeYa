namespace API_AprendeYa.Models
{
    public class CursoMatriculado
    {
        public int IdCurso { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }
        public DateTime FechaAdquisicion { get; set; }
    }
}