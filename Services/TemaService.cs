using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API_AprendeYa.Services
{
    public class TemaService : ITemaService
    {
        private readonly string _connection;

        public TemaService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }

        public List<Tema> GetTemas()
        {
            using var db = new SqlConnection(_connection);
            return db.Query<Tema>("SELECT * FROM tema").ToList();
        }

        public Tema GetTemaById(int idTema)
        {
            using var db = new SqlConnection(_connection);
            return db.QueryFirstOrDefault<Tema>(
                "SELECT * FROM tema WHERE id_tema = @id",
                new { id = idTema });
        }

        public bool InsertTema(Tema tema)
        {
            using var db = new SqlConnection(_connection);
            string sql = @"INSERT INTO tema (id_modulo, titulo, descripcion, orden)
                       VALUES (@IdModulo, @Titulo, @Descripcion, @Orden)";
            return db.Execute(sql, tema) > 0;
        }

        public bool UpdateTema(Tema tema)
        {
            using var db = new SqlConnection(_connection);
            string sql = @"UPDATE tema SET
                       id_modulo = @IdModulo,
                       titulo = @Titulo,
                       descripcion = @Descripcion,
                       orden = @Orden
                       WHERE id_tema = @IdTema";
            return db.Execute(sql, tema) > 0;
        }

        public bool DeleteTema(int idTema)
        {
            using var db = new SqlConnection(_connection);
            return db.Execute("DELETE FROM tema WHERE id_tema = @id", new { id = idTema }) > 0;
        }
    }
}
