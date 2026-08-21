using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificacion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class VerificarRespuestaRepositorio : BaseRepositorio, IVerificarRespuestaRepositorio
{
    public List<VerificarRespuesta> Listar(int verificarId)
    {
        List<VerificarRespuesta>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.verificarid, T1.categoriaid, T1.preguntaid, T1.descripcion, T1.valor, T2.orden, T2.descripcion, T3.categoriaid, T3.titulo, T3.descripcion, T3.orden ");
            _query.Append("FROM VerificarRespuesta T1 ");
            _query.Append("INNER JOIN Categoria T2 ON T1.categoriaid = T2.id ");
            _query.Append("INNER JOIN Pregunta T3 ON T1.preguntaid = T3.id ");
            _query.Append("WHERE T1.verificarid = @verificarid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("verificarid", verificarId);

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<VerificarRespuesta>();

                    while (rd.Read())
                        lista.Add(new VerificarRespuesta
                        {
                            VerificarId = rd.GetInt32(0),
                            CategoriaId = rd.GetInt32(1),
                            PreguntaId = rd.GetInt32(2),
                            Descripcion = rd.GetString(3),
                            Valor = rd.GetInt32(4),
                            Categoria = new Categoria
                            {
                                Id = rd.GetInt32(1),
                                Orden = rd.GetInt32(5),
                                Descripcion = rd.GetString(6)
                            },
                            Pregunta = new Pregunta
                            {
                                CategoriaId = rd.GetInt32(7),
                                Titulo = !rd.IsDBNull(8) ? rd.GetString(8) : null,
                                Descripcion = !rd.IsDBNull(9) ? rd.GetString(9) : null,
                                Orden = rd.GetInt32(10)
                            }
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

    public bool Guardar(VerificarRespuesta entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_VerificarRespuesta_Guardar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.VerificarId);
                cmd.Parameters.AddWithValue("p1", entidad.CategoriaId);
                cmd.Parameters.AddWithValue("p2", entidad.PreguntaId);
                cmd.Parameters.AddWithValue("p3", entidad.Descripcion);
                cmd.Parameters.AddWithValue("p4", entidad.Valor);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar la respuesta.", ex);
        }
    }

    #region Constructores

    public VerificarRespuestaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}