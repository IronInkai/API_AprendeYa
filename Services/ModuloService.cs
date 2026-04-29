using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API_AprendeYa.Services
{
    public class ModuloService : BaseService, IModuloService
    {
        //private readonly string _connection;

        public ModuloService(IConfiguration config) : base(config) { }

        public List<Modulo> GetModulos()
        {
            using var db = new SqlConnection(_connection);
            return db.Query<Modulo>("SELECT * FROM modulo").ToList();
        }

        public List<Modulo> GetModulosByCurso(int idCurso)
        {
            using var db = new SqlConnection(_connection);            
            string sql = @"SELECT id_modulo AS IdModulo, 
                          id_curso AS IdCurso, 
                          titulo AS Titulo, 
                          descripcion AS Descripcion, 
                          orden AS Orden 
                   FROM modulo 
                   WHERE id_curso = @id";
            return db.Query<Modulo>(sql, new { id = idCurso }).ToList();
        }

        public Modulo GetModuloById(int idModulo)
        {
            using var db = new SqlConnection(_connection);
            string sql = @"SELECT id_modulo AS IdModulo, id_curso AS IdCurso, 
                          titulo AS Titulo, descripcion AS Descripcion, orden AS Orden 
                   FROM modulo WHERE id_modulo = @id";
            return db.QueryFirstOrDefault<Modulo>(sql, new { id = idModulo });
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
