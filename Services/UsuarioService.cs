using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_AprendeYa.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public UsuarioService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ==========================================
        // AUTENTICACIÓN (LOGIN Y REGISTRO)
        // ==========================================

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

                if (usuario != null)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

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
                        expires: DateTime.Now.AddHours(2),
                        signingCredentials: credentials);

                    usuario.Token = new JwtSecurityTokenHandler().WriteToken(token);
                }

                return usuario;
            }
        }

        public async Task<bool> RegistrarUsuarioAsync(RegistroRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Nombres", request.Nombres);
                parameters.Add("@Apellidos", request.Apellidos);
                parameters.Add("@Correo", request.Correo);
                parameters.Add("@Telefono", request.Telefono);
                parameters.Add("@Username", request.Username);
                parameters.Add("@Password", request.Password);

                var result = await connection.ExecuteAsync(
                    "sp_registrar_usuario_completo",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        // ==========================================
        // CRUD DE USUARIOS (MANTENIMIENTO ADMIN)
        // ==========================================

        public List<UsuarioAdmin> GetUsuarios()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        u.id_usuario AS IdUsuario, u.id_persona AS IdPersona, 
                        u.id_rol AS IdRol, u.username AS Username, 
                        u.contrasena_literal AS ContrasenaLiteral, u.estado AS Estado,
                        p.nombres AS Nombres, p.apellidos AS Apellidos, p.correo AS Correo, p.telefono AS Telefono, r.nombre AS NombreRol 
                    FROM usuario u
                    LEFT JOIN persona p ON u.id_persona = p.id_persona
                    LEFT JOIN rol r ON u.id_rol = r.id_rol";

                return connection.Query<UsuarioAdmin>(sql).ToList();
            }
        }

        public UsuarioAdmin GetUsuarioById(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        u.id_usuario AS IdUsuario, u.id_persona AS IdPersona, 
                        u.id_rol AS IdRol, u.username AS Username, 
                        u.contrasena_literal AS ContrasenaLiteral, u.estado AS Estado,
                        p.nombres AS Nombres, p.apellidos AS Apellidos, p.correo AS Correo, p.telefono AS Telefono
                    FROM usuario u
                    LEFT JOIN persona p ON u.id_persona = p.id_persona
                    WHERE u.id_usuario = @id";

                return connection.QueryFirstOrDefault<UsuarioAdmin>(sql, new { id = idUsuario });
            }
        }

        public bool InsertUsuario(UsuarioAdmin usuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int? nuevoIdPersona = null;

                        // PASO 1: Insertar Persona (Si no es fantasma)
                        if (!string.IsNullOrWhiteSpace(usuario.Nombres))
                        {
                            string sqlPersona = @"
                                INSERT INTO persona (nombres, apellidos, correo, telefono)
                                OUTPUT INSERTED.id_persona
                                VALUES (@Nombres, @Apellidos, @Correo, @Telefono)";

                            nuevoIdPersona = connection.QuerySingle<int>(sqlPersona, new
                            {
                                Nombres = usuario.Nombres,
                                Apellidos = usuario.Apellidos,
                                Correo = usuario.Correo,
                                Telefono = usuario.Telefono
                            }, transaction);
                        }

                        // PASO 2: Insertar Usuario (Credenciales)
                        string sqlUsuario = @"
                            INSERT INTO usuario 
                            (id_persona, id_rol, username, contrasena_hash, contrasena_literal, estado, fecha_registro)
                            VALUES 
                            (@IdPersona, @IdRol, @Username, 
                            CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', CAST(@ContrasenaLiteral AS VARCHAR(255))), 2), 
                            @ContrasenaLiteral, @Estado, GETDATE())";

                        connection.Execute(sqlUsuario, new
                        {
                            IdPersona = nuevoIdPersona,
                            IdRol = usuario.IdRol,
                            Username = usuario.Username,
                            ContrasenaLiteral = usuario.ContrasenaLiteral,
                            Estado = usuario.Estado
                        }, transaction);

                        // =========================================================
                        // PASO 3: LA MAGIA DE LA HERENCIA (Insertar en tabla hija)
                        // =========================================================
                        // Solo insertamos en las tablas hijas si el usuario NO es fantasma
                        if (nuevoIdPersona.HasValue)
                        {
                            if (usuario.IdRol == 1) // Es Administrador
                            {
                                string sqlAdmin = "INSERT INTO administrador (id_persona, nivel_acceso) VALUES (@IdPersona, 'Total')";
                                connection.Execute(sqlAdmin, new { IdPersona = nuevoIdPersona }, transaction);
                            }
                            else if (usuario.IdRol == 2) // Es Alumno
                            {
                                string sqlAlumno = "INSERT INTO alumno (id_persona, fecha_registro, estado_academico) VALUES (@IdPersona, GETDATE(), 'Activo')";
                                connection.Execute(sqlAlumno, new { IdPersona = nuevoIdPersona }, transaction);
                            }
                            else if (usuario.IdRol == 3) // Es Profesor (Instructor)
                            {
                                // Le ponemos datos genéricos que luego él mismo podrá editar en su perfil
                                string sqlInstructor = "INSERT INTO instructor (id_persona, especialidad, descripcion) VALUES (@IdPersona, 'Sin especialidad', 'Perfil nuevo')";
                                connection.Execute(sqlInstructor, new { IdPersona = nuevoIdPersona }, transaction);
                            }
                        }

                        // Guardamos los cambios reales en la BD
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al insertar: " + ex.Message);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        // ==========================================
        // ACTUALIZAR USUARIO (Manejo de Persona Dinámico)
        // ==========================================
        public bool UpdateUsuario(UsuarioAdmin usuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // PASO 1: Manejar la tabla Persona
                        if (!string.IsNullOrWhiteSpace(usuario.Nombres))
                        {
                            if (usuario.IdPersona.HasValue && usuario.IdPersona > 0)
                            {
                                string sqlUpdatePersona = @"
                                    UPDATE persona SET nombres = @Nombres, apellidos = @Apellidos, 
                                    correo = @Correo, telefono = @Telefono 
                                    WHERE id_persona = @IdPersona";
                                connection.Execute(sqlUpdatePersona, usuario, transaction);
                            }
                            else
                            {
                                string sqlInsertPersona = @"
                                    INSERT INTO persona (nombres, apellidos, correo, telefono)
                                    OUTPUT INSERTED.id_persona
                                    VALUES (@Nombres, @Apellidos, @Correo, @Telefono)";
                                usuario.IdPersona = connection.QuerySingle<int>(sqlInsertPersona, usuario, transaction);
                            }
                        }

                        // PASO 2: Actualizar la tabla Usuario
                        string sqlUsuario = @"
                            UPDATE usuario SET 
                                id_persona = @IdPersona,
                                id_rol = @IdRol,
                                username = @Username,
                                estado = @Estado,
                                fecha_actualizacion = GETDATE()
                            WHERE id_usuario = @IdUsuario";

                        connection.Execute(sqlUsuario, usuario, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al actualizar: " + ex.Message);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool DeleteUsuario(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // En lugar de borrar la fila, simplemente apagamos el estado a 0 (Inactivo)
                // y actualizamos la fecha de modificación para tenaer auditoría.
                string sql = @"
                    UPDATE usuario 
                    SET estado = 0, 
                        fecha_actualizacion = GETDATE() 
                    WHERE id_usuario = @id";

                return connection.Execute(sql, new { id = idUsuario }) > 0;
            }
        }
        // ==========================================
        // MATRICULAR ALUMNO (Forzado por el Admin)
        // ==========================================
        // ==========================================
        // MATRICULAR ALUMNO (Beca / Asignación Forzada por Admin)
        // ==========================================
        public bool MatricularAlumno(int idUsuario, int idCurso)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Verificamos si ya tiene el curso comprado/asignado
                        string checkSql = @"
                            SELECT COUNT(1) 
                            FROM venta v
                            INNER JOIN venta_curso vc ON v.id_venta = vc.id_venta
                            WHERE v.id_usuario = @IdUsuario 
                              AND vc.id_curso = @IdCurso 
                              AND v.estado = 'Completado'";

                        int existe = connection.ExecuteScalar<int>(checkSql, new { IdUsuario = idUsuario, IdCurso = idCurso }, transaction);

                        if (existe > 0)
                        {
                            return false; // Ya tiene el curso
                        }

                        // 2. Creamos la "Venta" a costo 0
                        string sqlVenta = @"
                            INSERT INTO venta (id_usuario, fecha, total, estado)
                            OUTPUT INSERTED.id_venta
                            VALUES (@IdUsuario, GETDATE(), 0.00, 'Completado')";

                        int idVenta = connection.QuerySingle<int>(sqlVenta, new { IdUsuario = idUsuario }, transaction);

                        // 3. Vinculamos el curso a esa venta
                        string sqlVentaCurso = @"
                            INSERT INTO venta_curso (id_venta, id_curso, precio)
                            VALUES (@IdVenta, @IdCurso, 0.00)";

                        connection.Execute(sqlVentaCurso, new { IdVenta = idVenta, IdCurso = idCurso }, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al matricular: " + ex.Message);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        public List<CursoMatriculado> ObtenerCursosDeAlumno(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        c.id_curso AS IdCurso,
                        c.titulo AS Titulo,
                        c.descripcion AS Descripcion,
                        c.imagen_url AS ImagenUrl,
                        v.fecha AS FechaAdquisicion
                    FROM curso c
                    INNER JOIN venta_curso vc ON c.id_curso = vc.id_curso
                    INNER JOIN venta v ON vc.id_venta = v.id_venta
                    WHERE v.id_usuario = @IdUsuario AND v.estado = 'Completado'";

                return connection.Query<CursoMatriculado>(sql, new { IdUsuario = idUsuario }).ToList();
            }
        }
    }
}