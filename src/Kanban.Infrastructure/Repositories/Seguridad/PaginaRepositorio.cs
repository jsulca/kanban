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

public class PaginaRepositorio : BaseRepositorio, IPaginaRepositorio
{
    public PagedResult<Pagina> ListarPorPagina(PaginaFiltro? filter, int page, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Pagina>? lista = null;
        var totalRows = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.nombre, T1.area, T1.controlador, T1.accion FROM Pagina T1 ");
            if (filter != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (!string.IsNullOrEmpty(filter.Nombre))
                {
                    _queryConditions.Append("AND T1.nombre ILIKE '%' || @nombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("nombre", filter.Nombre));
                }

                if (!string.IsNullOrEmpty(filter.Area))
                {
                    _queryConditions.Append("AND T1.area ILIKE '%' || @area || '%' ");
                    _parametros.Add(new NpgsqlParameter("area", filter.Area));
                }

                if (!string.IsNullOrEmpty(filter.Controlador))
                {
                    _queryConditions.Append("AND T1.controlador ILIKE '%' || @controlador || '%' ");
                    _parametros.Add(new NpgsqlParameter("controlador", filter.Controlador));
                }

                if (!string.IsNullOrEmpty(filter.Accion))
                {
                    _queryConditions.Append("AND T1.accion ILIKE '%' || @accion || '%' ");
                    _parametros.Add(new NpgsqlParameter("accion", filter.Accion));
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
                        lista = new List<Pagina>();
                        while (rd.Read())
                            lista.Add(new Pagina
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Area = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                                Controlador = rd.GetString(3),
                                Accion = rd.GetString(4)
                            });
                    }

                    rd.Close();
                }

                _query = new StringBuilder();
                _query.Append("SELECT COUNT(T1.id) FROM Pagina T1 ");
                if (filter != null) _query.Append(_queryConditions);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _query.ToString();

                totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return new PagedResult<Pagina>(lista ?? [], totalRows, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar las paginas.", ex);
        }
    }

    public List<Pagina>? Listar()
    {
        List<Pagina>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, area, controlador, accion ");
            _query.Append("FROM Pagina ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.HasRows)
                    {
                        lista = new List<Pagina>();
                        while (rd.Read())
                            lista.Add(new Pagina
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Area = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                                Controlador = rd.GetString(3),
                                Accion = rd.GetString(4)
                            });
                    }

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar las paginas.", ex);
        }
    }

    public Pagina? Buscar(int id)
    {
        Pagina? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, area, controlador, accion ");
            _query.Append("FROM Pagina ");
            _query.Append("WHERE id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        entidad = new Pagina
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Area = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            Controlador = rd.GetString(3),
                            Accion = rd.GetString(4)
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

    public void Guardar(Pagina entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Accion);
        ArgumentNullException.ThrowIfNull(entidad.Controlador);
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Pagina_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Nombre);
                cmd.Parameters.AddWithValue("p1", entidad.Area ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Controlador);
                cmd.Parameters.AddWithValue("p3", entidad.Accion);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar la pagina", ex);
        }
    }

    public void Actualizar(Pagina entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Accion);
        ArgumentNullException.ThrowIfNull(entidad.Controlador);
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Pagina_Actualizar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre);
                cmd.Parameters.AddWithValue("p2", entidad.Area ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Controlador);
                cmd.Parameters.AddWithValue("p4", entidad.Accion);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al actualizar la pagina", ex);
        }
    }

    #region Constructores

    public PaginaRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}