using System.Text;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificaciones;

public class RespuestaRepositorio : BaseRepositorio, IRespuestaRepositorio
{
    public List<Respuesta> Listar(int verificacionId)
    {
        List<Respuesta> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.preguntaid, T1.valor, T1.descripcion ");
            _query.Append("FROM Respuesta T1 ");
            _query.Append("INNER JOIN Pregunta T2 ON T1.preguntaid= T2.id ");
            _query.Append("INNER JOIN Categoria T3 ON T2.categoriaid = T3.id ");
            _query.Append("WHERE T3.verificacionid = @verificacionid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("verificacionid", verificacionId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Respuesta
                        {
                            PreguntaId = rd.GetInt32(0),
                            Valor = rd.GetInt32(1),
                            Descripcion = !rd.IsDBNull(2) ? rd.GetString(2) : null
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las respuestas.", ex);
        }
    }

    public bool Guardar(Respuesta entidad)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Respuesta_Guardar(@p0, @p1, @p2)";

                cmd.Parameters.AddWithValue("p0", entidad.PreguntaId);
                cmd.Parameters.AddWithValue("p1", entidad.Valor);
                cmd.Parameters.AddWithValue("p2", entidad.Descripcion ?? _NullValue);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una respuesta.", ex);
        }
    }

    public bool Limpiar(int preguntaId)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Respuesta_Limpiar(@p0)";
                cmd.Parameters.AddWithValue("p0", preguntaId);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al limpiar respuestas.", ex);
        }
    }

    #region Constructores

    public RespuestaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}