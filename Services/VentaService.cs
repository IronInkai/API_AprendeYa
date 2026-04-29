using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;

namespace API_AprendeYa.Services
{
    public class VentaService : IVentaService
    {
        private readonly string _connectionString;

        public VentaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<ReporteVenta> ObtenerReporteDeVentas()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        v.id_venta AS IdVenta,
                        u.username AS Usuario,
                        ISNULL(p.nombres + ' ' + p.apellidos, 'Usuario Fantasma') AS Alumno,
                        v.fecha AS Fecha,
                        v.total AS Total,
                        v.estado AS Estado
                    FROM venta v
                    INNER JOIN usuario u ON v.id_usuario = u.id_usuario
                    LEFT JOIN persona p ON u.id_persona = p.id_persona
                    ORDER BY v.fecha DESC";

                return connection.Query<ReporteVenta>(sql).ToList();
            }
        }
    }
}