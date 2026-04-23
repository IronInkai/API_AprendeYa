using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API_AprendeYa.Services
{
    public class NivelService : BaseService, INivelService
    {
        //private readonly string _connection;

        public NivelService(IConfiguration config) : base(config) { }

        public List<Nivel> GetNiveles()
        {
            using var db = new SqlConnection(_connection);
            return db.Query<Nivel>("SELECT * FROM nivel").ToList();
        }

        public Nivel GetNivelById(int idNivel)
        {
            using var db = new SqlConnection(_connection);
            return db.QueryFirstOrDefault<Nivel>(
                "SELECT * FROM nivel WHERE id_nivel = @id",
                new { id = idNivel });
        }

        public bool InsertNivel(Nivel nivel)
        {
            using var db = new SqlConnection(_connection);
            string sql = "INSERT INTO nivel (nombre) VALUES (@Nombre)";
            return db.Execute(sql, nivel) > 0;
        }

        public bool UpdateNivel(Nivel nivel)
        {
            using var db = new SqlConnection(_connection);
            string sql = "UPDATE nivel SET nombre = @Nombre WHERE id_nivel = @IdNivel";
            return db.Execute(sql, nivel) > 0;
        }

        public bool DeleteNivel(int idNivel)
        {
            using var db = new SqlConnection(_connection);
            return db.Execute("DELETE FROM nivel WHERE id_nivel = @id", new { id = idNivel }) > 0;
        }
    }
}
