namespace API_AprendeYa.Models // (Usa Frontend_AprendeYa.Models en tu proyecto web)
{
    public class ItemCarrito
    {
        public int IdDetalle { get; set; }
        public int IdCurso { get; set; }
        public string Titulo { get; set; }
        public string ImagenUrl { get; set; }
        public decimal Precio { get; set; }
    }

    public class CarritoCompras
    {
        public int IdCarrito { get; set; }
        public decimal Total { get; set; }
        public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
    }
}