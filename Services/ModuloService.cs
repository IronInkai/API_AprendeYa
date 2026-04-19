using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API_AprendeYa.Services
{
    public class ModuloService : IModuloService
    {
        private readonly string _connection;

        public ModuloService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }

        public List<Modulo> GetModulos()
        {
            using var db = new SqlConnection(_connection);
            return db.Query<Modulo>("SELECT * FROM modulo").ToList();
        }

        public Modulo GetModuloById(int idModulo)
        {
            using var db = new SqlConnection(_connection);
            return db.QueryFirstOrDefault<Modulo>(
                "SELECT * FROM modulo WHERE id_modulo = @id",
                new { id = idModulo });
        }

        public bool InsertModulo(Modulo modulo)
        {
            using var db = new SqlConnection(_connection);
            string sql = @"INSERT INTO modulo (id_curso, titulo, descripcion, orden)
                       VALUES (@IdCurso, @Titulo, @Descripcion, @Orden)";
            return db.Execute(sql, modulo) > 0;
        }

        public bool UpdateModulo(Modulo modulo)
        {
            using var db = new SqlConnection(_connection);
            string sql = @"UPDATE modulo SET
                       id_curso = @IdCurso,
                       titulo = @Titulo,
                       descripcion = @Descripcion,
                       orden = @Orden
                       WHERE id_modulo = @IdModulo";
            return db.Execute(sql, modulo) > 0;
        }

        public bool DeleteModulo(int idModulo)
        {
            using var db = new SqlConnection(_connection);
            return db.Execute("DELETE FROM modulo WHERE id_modulo = @id", new { id = idModulo }) > 0;
        }
    }
}
