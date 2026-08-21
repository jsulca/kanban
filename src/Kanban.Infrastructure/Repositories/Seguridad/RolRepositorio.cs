using System.Data;
using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class RolRepositorio : BaseRepositorio, IRolRepositorio
{
    public PagedResult<Rol> ListarPorPagina(RolFiltro? filter, int page, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Rol>? lista = null;
        var totalRows = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.nombre, T1.activo FROM Rol T1 ");

            if (filter != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (!string.IsNullOrEmpty(filter.Nombre))
                {
                    _queryConditions.Append("AND T1.nombre ILIKE '%' || @nombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("nombre", filter.Nombre));
                }

                if (filter.Activo.HasValue)
                {
                    _queryConditions.Append("AND T1.activo = @activo ");
                    _parametros.Add(new NpgsqlParameter("activo", filter.Activo.Value));
                }

                _query.Append(_queryConditions);
            }

            _query.Append("ORDER BY T1.id ASC ");
            _query.Append("LIMIT @limite ");
            _query.Append("OFFSET @offset ");

            _parametros.Add(new NpgsqlParameter("limite", pageSize));
            _parametros.Add(new NpgsqlParameter("offset", pageSize * (page - 1)));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.HasRows)
                    {
                        lista = new List<Rol>();
                        while (rd.Read())
                            lista.Add(new Rol
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Activo = rd.GetBoolean(2)
                            });
                    }

                    rd.Close();
                }

                _query = new StringBuilder();
                _query.Append("SELECT COUNT(T1.id) FROM Rol T1 ");
                if (filter != null) _query.Append(_queryConditions);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _query.ToString();

                totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return new PagedResult<Rol>(lista ?? [], totalRows, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los roles.", ex);
        }
    }

    public List<Rol>? Listar()
    {
        List<Rol>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, activo ");
            _query.Append("FROM Rol ");
            _query.Append("WHERE activo = TRUE ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.HasRows)
                    {
                        lista = new List<Rol>();
                        while (rd.Read())
                            lista.Add(new Rol
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Activo = rd.GetBoolean(2)
                            });
                    }

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los roles.", ex);
        }
    }

    public Rol? BuscarPorId(int id)
    {
        Rol? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, activo ");
            _query.Append("FROM Rol ");
            _query.Append("WHERE id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        entidad = new Rol
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
            throw new RepositorioException("Error al buscar por id.", ex);
        }
    }

    public void Guardar(Rol entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Rol_Guardar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.Nombre);
                cmd.Parameters.AddWithValue("p1", entidad.Activo);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el rol", ex);
        }
    }

    public void Actualizar(Rol entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Rol_Actualizar(@p0, @p1, @p2)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre);
                cmd.Parameters.AddWithValue("p2", entidad.Activo);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al actualizar el rol", ex);
        }
    }

    #region Constructores

    public RolRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}