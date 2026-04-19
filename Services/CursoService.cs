using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API_AprendeYa.Services
{
    public class CursoService : ICursoService
    {
        private readonly IConfiguration _config;
        private readonly string _connection;

        public CursoService(IConfiguration config)
        {
            _config = config;
            _connection = _config.GetConnectionString("DefaultConnection");
        }

        public List<Curso> GetCursos()
        {
            using var db = new SqlConnection(_connection);
            return db.Query<Curso>("SELECT * FROM curso").ToList();
        }

        public Curso GetCursoById(int idCurso)
        {
            using var db = new SqlConnection(_connection);
            return db.QueryFirstOrDefault<Curso>(
                "SELECT * FROM curso WHERE id_curso = @id",
                new { id = idCurso });
        }

        public bool InsertCurso(Curso curso)
        {
            using var db = new SqlConnection(_connection);

            string sql = @"INSERT INTO curso 
            (titulo, descripcion, precio, id_nivel, id_instructor, id_categoria, imagen_url, estado)
            VALUES (@Titulo, @Descripcion, @Precio, @IdNivel, @IdInstructor, @IdCategoria, @ImagenUrl, @Estado)";

            return db.Execute(sql, curso) > 0;
        }

        public bool UpdateCurso(Curso curso)
        {
            using var db = new SqlConnection(_connection);

            string sql = @"UPDATE curso SET 
                titulo = @Titulo,
                descripcion = @Descripcion,
                precio = @Precio,
                id_nivel = @IdNivel,
                id_instructor = @IdInstructor,
                id_categoria = @IdCategoria,
                imagen_url = @ImagenUrl,
                estado = @Estado
                WHERE id_curso = @IdCurso";

            return db.Execute(sql, curso) > 0;
        }

        public bool DeleteCurso(int idCurso)
        {
            using var db = new SqlConnection(_connection);
            return db.Execute(
                "DELETE FROM curso WHERE id_curso = @id",
                new { id = idCurso }) > 0;
        }

        //FILTRO
        public async Task<IEnumerable<Curso>> FiltrarCursos(int? nivel, int? categoria)
        {
            using var db = new SqlConnection(_connection);

            string sql = @"SELECT * FROM curso
                           WHERE (@nivel IS NULL OR id_nivel = @nivel)
                           AND (@categoria IS NULL OR id_categoria = @categoria)";

            return await db.QueryAsync<Curso>(sql, new { nivel, categoria });
        }
    }
}
