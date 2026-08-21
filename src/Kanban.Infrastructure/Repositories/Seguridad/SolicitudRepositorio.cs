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

public class SolicitudRepositorio : BaseRepositorio, ISolicitudRepositorio
{
    public PagedResult<Solicitud> ListarPorPagina(SolicitudFiltro? filter, int page, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Solicitud> lista = new();
        var totalRows = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.nombre, T1.apellido, T1.nrodocumento, T1.correo, T1.telefono, T1.fecharegistro FROM Solicitud T1 ");

            if (filter != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (!string.IsNullOrEmpty(filter.Nombre))
                {
                    _queryConditions.Append("AND T1.nombre ILIKE '%' || @nombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("nombre", filter.Nombre));
                }

                if (!string.IsNullOrEmpty(filter.Apellido))
                {
                    _queryConditions.Append("AND T1.apellido ILIKE '%' || @apellido || '%' ");
                    _parametros.Add(new NpgsqlParameter("apellido", filter.Nombre));
                }

                if (!string.IsNullOrEmpty(filter.NroDocumento))
                {
                    _queryConditions.Append("AND T1.nrodocumento ILIKE '%' || @nrodocumento || '%' ");
                    _parametros.Add(new NpgsqlParameter("nrodocumento", filter.Nombre));
                }

                if (!string.IsNullOrEmpty(filter.Correo))
                {
                    _queryConditions.Append("AND T1.correo ILIKE '%' || @correo || '%' ");
                    _parametros.Add(new NpgsqlParameter("correo", filter.Nombre));
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
                        while (rd.Read())
                            lista.Add(new Solicitud
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Apellido = rd.GetString(2),
                                NroDocumento = rd.GetString(3),
                                Correo = rd.GetString(4),
                                Telefono = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                                FechaRegistro = rd.GetDateTime(6)
                            });

                    rd.Close();
                }

                _query = new StringBuilder();
                _query.Append("SELECT COUNT(T1.id) FROM Solicitud T1 ");
                if (filter != null) _query.Append(_queryConditions);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _query.ToString();

                totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return new PagedResult<Solicitud>(lista, totalRows, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar las solicitudes.", ex);
        }
    }

    public void Guardar(Solicitud entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Apellido);
        ArgumentNullException.ThrowIfNull(entidad.Correo);
        ArgumentNullException.ThrowIfNull(entidad.Nombre);
        ArgumentNullException.ThrowIfNull(entidad.NroDocumento);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Solicitud_Guardar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Nombre);
                cmd.Parameters.AddWithValue("p1", entidad.Apellido);
                cmd.Parameters.AddWithValue("p2", entidad.NroDocumento);
                cmd.Parameters.AddWithValue("p3", entidad.Correo);
                cmd.Parameters.AddWithValue("p4", entidad.Telefono ?? _NullValue);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar la solicitud", ex);
        }
    }

    #region Constructores

    public SolicitudRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}