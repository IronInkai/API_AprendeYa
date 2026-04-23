using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API_AprendeYa.Services
{
    public class ForoService : IForoService
    {
        private readonly string _connection;

        public ForoService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }


        public List<TemaForo> GetPreguntas()
        {
            using var db = new SqlConnection(_connection);
            return db.Query<TemaForo>("SELECT * FROM tema_foro").ToList();
        }


        public TemaForo GetPreguntaById(int idTema)
        {
            using var db = new SqlConnection(_connection);
            return db.QueryFirstOrDefault<TemaForo>(
                "SELECT * FROM tema_foro WHERE id_tema = @id",
                new { id = idTema });
        }


        public bool CrearPregunta(TemaForo pregunta)
        {
            using var db = new SqlConnection(_connection);

            string sql = @"INSERT INTO tema_foro 
            (id_foro, id_usuario, titulo, contenido, imagen_url)
            VALUES (@IdForo, @IdUsuario, @Titulo, @Contenido, @ImagenUrl)";

            return db.Execute(sql, pregunta) > 0;
        }


        public List<RespuestaForo> GetRespuestas(int idTema)
        {
            using var db = new SqlConnection(_connection);
            return db.Query<RespuestaForo>(
                "SELECT * FROM respuesta_foro WHERE id_tema = @id",
                new { id = idTema }).ToList();
        }


        public bool CrearRespuesta(RespuestaForo respuesta)
        {
            using var db = new SqlConnection(_connection);

            string sql = @"INSERT INTO respuesta_foro 
            (id_tema, id_usuario, contenido, imagen_url)
            VALUES (@IdTema, @IdUsuario, @Contenido, @ImagenUrl)";

            return db.Execute(sql, respuesta) > 0;
        }


        public bool DarLike(int idRespuesta, int idUsuario, bool tipo)
        {
            using var db = new SqlConnection(_connection);

            string sql = @"INSERT INTO voto_respuesta (id_respuesta, id_usuario, tipo)
                           VALUES (@IdRespuesta, @IdUsuario, @Tipo)";

            return db.Execute(sql, new
            {
                IdRespuesta = idRespuesta,
                IdUsuario = idUsuario,
                Tipo = tipo
            }) > 0;
        }


        public bool MarcarMejorRespuesta(int idRespuesta)
        {
            using var db = new SqlConnection(_connection);

            string sql = @"UPDATE respuesta_foro
                           SET es_mejor_respuesta = 1
                           WHERE id_respuesta = @IdRespuesta";

            return db.Execute(sql, new { IdRespuesta = idRespuesta }) > 0;
        }


        public List<TemaForo> GetPreguntasConRespuestas()
        {
            using var db = new SqlConnection(_connection);

    
            var temas = db.Query<TemaForo, Usuario, TemaForo>(
                @"SELECT t.*, u.*
                  FROM tema_foro t
                  INNER JOIN usuario u ON t.id_usuario = u.id_usuario",
                (tema, usuario) =>
                {
                    tema.Usuario = usuario;
                    return tema;
                },
                splitOn: "id_usuario"
            ).ToList();

            foreach (var tema in temas)
            {
                tema.Respuestas = db.Query<RespuestaForo>(
                    "SELECT * FROM respuesta_foro WHERE id_tema = @id",
                    new { id = tema.IdTema }
                ).ToList();
            }

            return temas;
        }
    }
}