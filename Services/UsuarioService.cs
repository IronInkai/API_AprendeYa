using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens; // NUEVO
using System.Data;
using System.IdentityModel.Tokens.Jwt; // NUEVO
using System.Security.Claims; // NUEVO
using System.Text; // NUEVO

namespace API_AprendeYa.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration; // NUEVO: Para leer appsettings.json

        public UsuarioService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ... tu método RegistrarUsuarioAsync se queda igual ...

        public async Task<UsuarioSesion> LoginAsync(LoginRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
    SELECT u.id_usuario as IdUsuario, u.username, 
           p.nombres + ' ' + p.apellidos as NombreCompleto, u.id_rol as IdRol
    FROM usuario u
    INNER JOIN persona p ON u.id_persona = p.id_persona
    WHERE u.username = @Username 
      AND u.contrasena_hash = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', CAST(@Password AS VARCHAR(255))), 2)
      AND u.estado = 1";

                var usuario = await connection.QueryFirstOrDefaultAsync<UsuarioSesion>(sql, new
                {
                    Username = request.Username,
                    Password = request.Password
                });

                // Si el usuario existe, generamos el Token
                if (usuario != null)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    // Agregamos la información (claims) que irá dentro del token
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Name, usuario.Username),
                        new Claim(ClaimTypes.Role, usuario.IdRol.ToString())
                    };

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddHours(2), // El token expira en 2 horas
                        signingCredentials: credentials);

                    usuario.Token = new JwtSecurityTokenHandler().WriteToken(token);
                }

                return usuario;
            }
        }

        public Task<bool> RegistrarUsuarioAsync(RegistroRequest request)
        {
            throw new NotImplementedException();
        }
    }
}