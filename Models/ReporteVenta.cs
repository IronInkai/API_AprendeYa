namespace API_AprendeYa.Models
{
    public class ReporteVenta
    {
        public int IdVenta { get; set; }
        public string Usuario { get; set; }
        public string Alumno { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}