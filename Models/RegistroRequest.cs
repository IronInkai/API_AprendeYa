namespace API_AprendeYa.Models
{
    public class RegistroRequest
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }

        // Datos de Usuario 
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
