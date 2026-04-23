using API_AprendeYa.Models;

namespace API_AprendeYa.Services.Interfaces
{
    public interface IForoService
    {
        List<TemaForo> GetPreguntasConRespuestas();
        List<TemaForo> GetPreguntas();
        TemaForo GetPreguntaById(int idTema);
        bool CrearPregunta(TemaForo pregunta);

        List<RespuestaForo> GetRespuestas(int idTema);
        bool CrearRespuesta(RespuestaForo respuesta);

        bool DarLike(int idRespuesta, int idUsuario, bool tipo);
        bool MarcarMejorRespuesta(int idRespuesta);
    }
}
