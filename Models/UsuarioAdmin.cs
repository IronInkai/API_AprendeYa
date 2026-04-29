namespace API_AprendeYa.Models
{
    public class UsuarioAdmin
    {
        // === Campos de la tabla Usuario ===
        public int IdUsuario { get; set; }
        public int? IdPersona { get; set; }
        public int? IdRol { get; set; }
        public string? Username { get; set; } // Agregado ?
        public string? ContrasenaLiteral { get; set; } // Agregado ?
        public bool Estado { get; set; }

        // === Extras que Dapper llenará con el LEFT JOIN ===
        public string? Nombres { get; set; } // Agregado ?
        public string? Apellidos { get; set; } // Agregado ?
        public string? Correo { get; set; } // Agregado ?
        public string? Telefono { get; set; } // ¡El campo que faltaba!
        public string? NombreRol { get; set; } // Agregado ? <-- Este era el culpable
    }
}