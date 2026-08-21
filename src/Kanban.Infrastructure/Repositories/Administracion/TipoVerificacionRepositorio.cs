using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class TipoVerificacionRepositorio : BaseRepositorio, ITipoVerificacionRepositorio
{
    public List<TipoVerificacion> Listar(TipoVerificacionFiltro? filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<TipoVerificacion> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, activo, vp, igp FROM TipoVerificacion ");

            if (filtro != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (!string.IsNullOrEmpty(filtro.Nombre))
                {
                    _queryConditions.Append("AND nombre ILIKE '%' || @p_nombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("nombre", filtro.Nombre));
                }

                if (filtro.Activo.HasValue)
                {
                    _queryConditions.Append("AND activo = @activo ");
                    _parametros.Add(new NpgsqlParameter("activo", filtro.Activo.Value));
                }

                _query.Append(_queryConditions);
            }

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new TipoVerificacion
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Activo = rd.GetBoolean(2),
                            VP = rd.GetBoolean(3),
                            IGP = rd.GetBoolean(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los tipos de verificacion.", ex);
        }
    }

    public TipoVerificacion? Buscar(int id)
    {
        TipoVerificacion? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, activo, vp, igp ");
            _query.Append("FROM TipoVerificacion ");
            _query.Append("WHERE id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new TipoVerificacion
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Activo = rd.GetBoolean(2),
                            VP = rd.GetBoolean(3),
                            IGP = rd.GetBoolean(4)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar un tipo de verificacion.", ex);
        }
    }

    public bool Guardar(TipoVerificacion entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_TipoVerificacion_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Nombre.ToUpper());
                cmd.Parameters.AddWithValue("p1", entidad.Activo);
                cmd.Parameters.AddWithValue("p2", entidad.VP);
                cmd.Parameters.AddWithValue("p3", entidad.IGP);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar un origen.", ex);
        }
    }

    public bool Actualizar(TipoVerificacion entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_TipoVerificacion_Actualizar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre.ToUpper());
                cmd.Parameters.AddWithValue("p2", entidad.Activo);
                cmd.Parameters.AddWithValue("p3", entidad.VP);
                cmd.Parameters.AddWithValue("p4", entidad.IGP);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar una verificacion.", ex);
        }
    }

    #region Constructores

    public TipoVerificacionRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}