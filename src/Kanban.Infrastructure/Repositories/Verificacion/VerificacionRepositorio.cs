using System.Data;
using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class VerificacionRepositorio : BaseRepositorio, IVerificacionRepositorio
{
    public PagedResult<Domain.Genericos.Verificacion.Verificacion> ListarPorPagina(VerificacionFiltro? filtro, int page, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Domain.Genericos.Verificacion.Verificacion> lista = new();
        var totalRows = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.nombre, T1.activo, T1.rom, T1.tipoverificacionid, T2.nombre ");
            _query.Append("FROM Verificacion T1 ");
            _query.Append("INNER JOIN TipoVerificacion T2 ON T1.tipoverificacionid = T2.id ");

            if (filtro != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (!string.IsNullOrEmpty(filtro.Nombre))
                {
                    _queryConditions.Append("AND T1.nombre ILIKE '%' || @nombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("nombre", filtro.Nombre));
                }

                if (!string.IsNullOrEmpty(filtro.TipoVerificacionNombre))
                {
                    _queryConditions.Append("AND T2.nombre ILIKE '%' || @tipoverificacionnombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("tipoverificacionnombre", filtro.TipoVerificacionNombre));
                }

                if (filtro.Activo.HasValue)
                {
                    _queryConditions.Append("AND T1.activo = @activo ");
                    _parametros.Add(new NpgsqlParameter("activo", filtro.Activo.Value));
                }

                if (filtro.Rom.HasValue)
                {
                    _queryConditions.Append("AND T1.rom = @rom ");
                    _parametros.Add(new NpgsqlParameter("rom", filtro.Rom.Value));
                }

                _query.Append(_queryConditions);
            }

            _query.Append("ORDER BY T1.id DESC ");
            _query.Append("LIMIT @limite ");
            _query.Append("OFFSET @hasta ");

            _parametros.Add(new NpgsqlParameter("limite", pageSize));
            _parametros.Add(new NpgsqlParameter("hasta", pageSize * (page - 1)));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Domain.Genericos.Verificacion.Verificacion
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Activo = rd.GetBoolean(2),
                            Rom = rd.GetBoolean(3),
                            TipoVerificacionId = rd.GetInt32(4),
                            TipoVerificacion = new TipoVerificacion { Nombre = rd.GetString(5) }
                        });
                    rd.Close();
                }

                _query = new StringBuilder();
                _query.Append("SELECT COUNT(T1.id) ");
                _query.Append("FROM Verificacion T1 ");
                _query.Append("INNER JOIN TipoVerificacion T2 ON T1.tipoverificacionid = T2.id ");

                if (filtro != null) _query.Append(_queryConditions);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _query.ToString();

                totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return new PagedResult<Domain.Genericos.Verificacion.Verificacion>(lista, totalRows, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de listar las verificaciones por pagina.", ex);
        }
    }

    public List<Domain.Genericos.Verificacion.Verificacion> Listar()
    {
        List<Domain.Genericos.Verificacion.Verificacion> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.nombre, T1.activo, T1.rom, T1.tipoverificacionid, T2.nombre, T1.vp ");
            _query.Append("FROM Verificacion T1 ");
            _query.Append("INNER JOIN TipoVerificacion T2 ON T1.tipoverificacionid = T2.id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Domain.Genericos.Verificacion.Verificacion
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
            throw new RepositorioException("Ocurrió un problema al listar las verificaciones.", ex);
        }
    }

    public Domain.Genericos.Verificacion.Verificacion? Buscar(int id)
    {
        Domain.Genericos.Verificacion.Verificacion? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.nombre, T1.activo, T1.rom, T1.fortaleza, T1.oportunidad, T1.planaccion, T1.instruccion, T1.instructivoestandar, T1.resumencategoria, T1.tipoverificacionid, T2.nombre, T1.vp, T1.igp ");
            _query.Append("FROM Verificacion T1 ");
            _query.Append("INNER JOIN TipoVerificacion T2 ON T1.tipoverificacionid = T2.id ");
            _query.Append("WHERE T1.id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Domain.Genericos.Verificacion.Verificacion
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Activo = rd.GetBoolean(2),
                            Rom = rd.GetBoolean(3),
                            Fortaleza = rd.GetBoolean(4),
                            Oportunidad = rd.GetBoolean(5),
                            PlanAccion = rd.GetBoolean(6),
                            Instruccion = !rd.IsDBNull(7) ? rd.GetString(7) : null,
                            InstructivoEstandar = rd.GetBoolean(8),
                            ResumenCategoria = rd.GetBoolean(9),
                            TipoVerificacionId = rd.GetInt32(10),
                            TipoVerificacion = new TipoVerificacion { Nombre = rd.GetString(11) },
                            VP = rd.GetBoolean(12),
                            IGP = rd.GetBoolean(13)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar una verificacion.", ex);
        }
    }

    public bool Guardar(Domain.Genericos.Verificacion.Verificacion entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM usp_Verificacion_Guardar(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)";

                cmd.Parameters.AddWithValue("p0", entidad.Nombre);
                cmd.Parameters.AddWithValue("p1", entidad.Instruccion ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Rom);
                cmd.Parameters.AddWithValue("p3", entidad.Activo);
                cmd.Parameters.AddWithValue("p4", entidad.Fortaleza);
                cmd.Parameters.AddWithValue("p5", entidad.Oportunidad);
                cmd.Parameters.AddWithValue("p6", entidad.PlanAccion);
                cmd.Parameters.AddWithValue("p7", entidad.InstructivoEstandar);
                cmd.Parameters.AddWithValue("p8", entidad.ResumenCategoria);
                cmd.Parameters.AddWithValue("p9", entidad.TipoVerificacionId);
                cmd.Parameters.AddWithValue("p10", entidad.VP);
                cmd.Parameters.AddWithValue("p11", entidad.IGP);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una verificacion.", ex);
        }
    }

    public void Actualizar(Domain.Genericos.Verificacion.Verificacion entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText =
                    "CALL usp_Verificacion_Actualizar(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre);
                cmd.Parameters.AddWithValue("p2", entidad.Activo);
                cmd.Parameters.AddWithValue("p3", entidad.Instruccion ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.Rom);
                cmd.Parameters.AddWithValue("p5", entidad.Fortaleza);
                cmd.Parameters.AddWithValue("p6", entidad.Oportunidad);
                cmd.Parameters.AddWithValue("p7", entidad.PlanAccion);
                cmd.Parameters.AddWithValue("p8", entidad.InstructivoEstandar);
                cmd.Parameters.AddWithValue("p9", entidad.ResumenCategoria);
                cmd.Parameters.AddWithValue("p10", entidad.TipoVerificacionId);
                cmd.Parameters.AddWithValue("p11", entidad.VP);
                cmd.Parameters.AddWithValue("p12", entidad.IGP);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar una verificacion.", ex);
        }
    }

    #region Constructores

    public VerificacionRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}