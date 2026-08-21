using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificacion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class PreguntaRepositorio : BaseRepositorio, IPreguntaRepositorio
{
    public List<Pregunta> Listar(int verificacionId)
    {
        List<Pregunta> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.categoriaid, T1.orden, T1.titulo, T1.descripcion, T1.eliminado ");
            _query.Append("FROM Pregunta T1 ");
            _query.Append("INNER JOIN Categoria T2 ON T1.categoriaid = T2.id ");
            _query.Append("WHERE T2.verificacionid = @verificacionid ");
            _query.Append("AND T1.eliminado = FALSE ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("verificacionid", verificacionId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Pregunta
                        {
                            Id = rd.GetInt32(0),
                            CategoriaId = rd.GetInt32(1),
                            Orden = rd.GetInt32(2),
                            Titulo = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            Descripcion = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                            Eliminado = rd.GetBoolean(5)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las preguntas.", ex);
        }
    }

    public bool Guardar(Pregunta entidad)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Pregunta_Guardar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.CategoriaId);
                cmd.Parameters.AddWithValue("p1", entidad.Orden);
                cmd.Parameters.AddWithValue("p2", entidad.Titulo ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Descripcion ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.Eliminado);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una pregunta.", ex);
        }
    }

    public bool Actualizar(Pregunta entidad)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Pregunta_Actualizar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Orden);
                cmd.Parameters.AddWithValue("p2", entidad.Titulo ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Descripcion ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.Eliminado);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar una pregunta.", ex);
        }
    }

    #region Constructores

    public PreguntaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}