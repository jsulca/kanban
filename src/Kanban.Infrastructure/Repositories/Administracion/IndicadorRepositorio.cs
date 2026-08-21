using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class IndicadorRepositorio : BaseRepositorio, IIndicadorRepositorio
{
    public List<Indicador> Listar(IndicadorFiltro? filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Indicador> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, activo FROM Indicador ");
            if (filtro != null)
            {
                _query.Append("WHERE 1 = 1 ");

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
                        lista.Add(new Indicador
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Activo = rd.GetBoolean(2)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los indicadores.", ex);
        }
    }

    public Indicador? Buscar(int id)
    {
        Indicador? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, activo ");
            _query.Append("FROM Indicador ");
            _query.Append("WHERE id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Indicador
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Activo = rd.GetBoolean(2)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar un indicador.", ex);
        }
    }

    public bool Guardar(Indicador entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Indicador_Guardar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.Nombre.ToUpper());
                cmd.Parameters.AddWithValue("p1", entidad.Activo);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar un indicador.", ex);
        }
    }

    public bool Actualizar(Indicador entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Indicador_Actualizar(@p0, @p1, @p2)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre.ToUpper());
                cmd.Parameters.AddWithValue("p2", entidad.Activo);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar un indicador.", ex);
        }
    }

    #region Constructores

    public IndicadorRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}