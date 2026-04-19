using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API_AprendeYa.Services
{
    public class ContenidoService : IContenidoService
    {
        private readonly string _connection;

        public ContenidoService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }

        public List<Contenido> GetContenidos()
        {
            using var db = new SqlConnection(_connection);
            return db.Query<Contenido>("SELECT * FROM contenido").ToList();
        }

        public Contenido GetContenidoById(int idContenido)
        {
            using var db = new SqlConnection(_connection);
            return db.QueryFirstOrDefault<Contenido>(
                "SELECT * FROM contenido WHERE id_contenido = @id",
                new { id = idContenido });
        }

        public bool InsertContenido(Contenido contenido)
        {
            using var db = new SqlConnection(_connection);
            string sql = @"INSERT INTO contenido (id_tema, tipo, url, texto, duracion, orden)
                       VALUES (@IdTema, @Tipo, @Url, @Texto, @Duracion, @Orden)";
            return db.Execute(sql, contenido) > 0;
        }

        public bool UpdateContenido(Contenido contenido)
        {
            using var db = new SqlConnection(_connection);
            string sql = @"UPDATE contenido SET
                       id_tema = @IdTema,
                       tipo = @Tipo,
                       url = @Url,
                       texto = @Texto,
                       duracion = @Duracion,
                       orden = @Orden
                       WHERE id_contenido = @IdContenido";
            return db.Execute(sql, contenido) > 0;
        }

        public bool DeleteContenido(int idContenido)
        {
            using var db = new SqlConnection(_connection);
            return db.Execute("DELETE FROM contenido WHERE id_contenido = @id", new { id = idContenido }) > 0;
        }
    }
}
