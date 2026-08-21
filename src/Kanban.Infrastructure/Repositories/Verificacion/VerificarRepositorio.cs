using System.Data;
using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Verificacion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class VerificarRepositorio : BaseRepositorio, IVerificarRepositorio
{
    public PagedResult<Verificar> ListarPorPagina(VerificarFiltro? filter, int page, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Verificar>? lista = null;
        var totalRows = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.empleadoid, T1.verificacionid, T1.encargado, T1.fecharegistro, T1.puntajemaximo, T1.puntajeobtenido, T2.nombre, ");
            _query.Append(
                "T3.apellidopaterno, T3.apellidomaterno, T3.nombre, T4.id, T4.descripcion, T5.id, T5.descripcion, T1.vp ");
            _query.Append("FROM Verificar T1 ");
            _query.Append("INNER JOIN Verificacion T2 ON T1.verificacionid = T2.id ");
            _query.Append("INNER JOIN Empleado T3 ON T1.empleadoid = T3.id ");
            _query.Append("INNER JOIN Estructura T4 ON T1.tableroid = T4.id ");
            _query.Append("INNER JOIN Estructura T5 ON T1.estructuraid = T5.id ");

            if (filter != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (!string.IsNullOrEmpty(filter.VerificacionNombre))
                {
                    _queryConditions.Append("AND T2.nombre ILIKE '%' || @verificacionnombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("verificacionnombre", filter.VerificacionNombre));
                }

                if (!string.IsNullOrEmpty(filter.EmpleadoNombre))
                {
                    _queryConditions.Append("AND T3.nombre ILIKE '%' || @empleadonombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("empleadonombre", filter.EmpleadoNombre));
                }

                if (!string.IsNullOrEmpty(filter.EmpleadoApellidoPaterno))
                {
                    _queryConditions.Append("AND T3.apellidopaterno ILIKE '%' || @empleadoapellidopaterno || '%' ");
                    _parametros.Add(new NpgsqlParameter("empleadoapellidopaterno", filter.EmpleadoApellidoPaterno));
                }

                if (!string.IsNullOrEmpty(filter.EmpleadoApellidoMaterno))
                {
                    _queryConditions.Append("AND T3.apellidomaterno ILIKE '%' || @empleadoapellidomaterno || '%' ");
                    _parametros.Add(new NpgsqlParameter("empleadoapellidomaterno", filter.EmpleadoApellidoMaterno));
                }

                if (!string.IsNullOrEmpty(filter.Encargado))
                {
                    _queryConditions.Append("AND T1.encargado ILIKE '%' || @encargado || '%' ");
                    _parametros.Add(new NpgsqlParameter("encargado", filter.Encargado));
                }

                if (filter.EmpleadoId.HasValue)
                {
                    _queryConditions.Append("AND T1.empleadoid = @empleadoid ");
                    _parametros.Add(new NpgsqlParameter("empleadoid", filter.EmpleadoId.Value));
                }

                if (!string.IsNullOrEmpty(filter.EstructuraDescripcion))
                {
                    _queryConditions.Append("AND T5.descripcion ILIKE '%' || @estructuradescripcion || '%' ");
                    _parametros.Add(new NpgsqlParameter("estructuradescripcion", filter.EstructuraDescripcion));
                }

                if (filter.VP.HasValue)
                {
                    _queryConditions.Append("AND T1.vp = @vp ");
                    _parametros.Add(new NpgsqlParameter("vp", filter.VP.Value));
                }

                if (!string.IsNullOrEmpty(filter.TableroDescripcion))
                {
                    _queryConditions.Append("AND T4.descripcion ILIKE '%' || @tablerodescripcion || '%' ");
                    _parametros.Add(new NpgsqlParameter("tablerodescripcion", filter.TableroDescripcion));
                }


                _query.Append(_queryConditions);
            }

            _query.Append("ORDER BY T1.id DESC ");
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
                        lista = new List<Verificar>();
                        while (rd.Read())
                            lista.Add(new Verificar
                            {
                                Id = rd.GetInt32(0),
                                EmpleadoId = rd.GetInt32(1),
                                VerificacionId = rd.GetInt32(2),
                                Encargado = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                                FechaRegistro = rd.GetDateTime(4),
                                PuntajeMaximo = rd.GetInt32(5),
                                PuntajeObtenido = rd.GetInt32(6),
                                Verificacion = new Domain.Genericos.Verificacion.Verificacion { Nombre = rd.GetString(7) },
                                Empleado = new Empleado
                                {
                                    ApellidoPaterno = rd.GetString(8),
                                    ApellidoMaterno = rd.GetString(9),
                                    Nombre = rd.GetString(10)
                                },
                                TableroId = rd.GetInt32(11),
                                Tablero = new Estructura { Id = rd.GetInt32(11), Descripcion = rd.GetString(12) },
                                EstructuraId = rd.GetInt32(13),
                                Estructura = new Estructura { Id = rd.GetInt32(13), Descripcion = rd.GetString(14) },
                                VP = rd.GetBoolean(15)
                            });
                    }

                    rd.Close();
                }

                _query = new StringBuilder();
                _query.Append("SELECT COUNT(T1.id) FROM Verificar T1 ");
                _query.Append("INNER JOIN Verificacion T2 ON T1.verificacionid = T2.id ");
                _query.Append("INNER JOIN Empleado T3 ON T1.empleadoid = T3.id ");
                _query.Append("INNER JOIN Estructura T4 ON T1.tableroid = T4.id ");
                _query.Append("INNER JOIN Estructura T5 ON T1.estructuraid = T5.id ");

                if (filter != null) _query.Append(_queryConditions);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _query.ToString();

                totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return new PagedResult<Verificar>(lista ?? [], totalRows, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar las listas de confirmaciones.", ex);
        }
    }

    public List<Verificar> TableroResumen(VerificarFiltro filter)
    {
        // la consulta filtra siempre por tablero y rango de fechas, no son opcionales aqui
        ArgumentNullException.ThrowIfNull(filter.TableroId);
        ArgumentNullException.ThrowIfNull(filter.Desde);
        ArgumentNullException.ThrowIfNull(filter.Hasta);

        List<Verificar> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.empleadoid, T1.puntajeobtenido, T1.puntajemaximo, T1.vp, T2.tipoverificacionid, T2.vp, T1.fecharegistro, T1.areaid, T1.igp ");
            _query.Append("FROM Verificar T1 ");
            _query.Append("INNER JOIN Verificacion T2 ON T1.verificacionid = T2.id ");
            _query.Append("WHERE T1.tableroid = @tableroid ");
            _query.Append("AND T1.fecharegistro BETWEEN @desde AND @hasta ");
            _query.Append("AND T1.empleadoid::TEXT IN (SELECT * FROM regexp_split_to_table(@empleadoids, ',')) ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("tableroid", filter.TableroId.Value);
                cmd.Parameters.AddWithValue("desde", filter.Desde.Value);
                cmd.Parameters.AddWithValue("hasta", filter.Hasta.Value.AddDays(1));
                cmd.Parameters.AddWithValue("empleadoids",
                    filter.EmpleadoIds != null && filter.EmpleadoIds.Length > 0
                        ? string.Join(",", filter.EmpleadoIds)
                        : "0");

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Verificar
                        {
                            Id = rd.GetInt32(0),
                            EmpleadoId = rd.GetInt32(1),
                            PuntajeObtenido = rd.GetInt32(2),
                            PuntajeMaximo = rd.GetInt32(3),
                            VP = rd.GetBoolean(4),
                            Verificacion = new Domain.Genericos.Verificacion.Verificacion
                                { TipoVerificacionId = rd.GetInt32(5), VP = rd.GetBoolean(6) },
                            FechaRegistro = rd.GetDateTime(7),
                            AreaId = !rd.IsDBNull(8) ? rd.GetInt32(8) : (int?)null,
                            IGP = rd.GetBoolean(9)
                        });

                    rd.Close();
                }

                return lista;
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un error al momento de listar el resumen.", ex);
        }
    }

    public List<Verificar> Reporte(int tableroId, DateTime fechaDesde, DateTime fechaHasta)
    {
        List<Verificar> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.empleadoid, T1.verificacionid, T1.encargado, T1.fecharegistro, T1.puntajemaximo, T1.puntajeobtenido, T2.nombre, T3.apellidopaterno, T3.apellidomaterno, T3.nombre, ");
            _query.Append(
                "T4.id, T4.descripcion, T5.id, T5.descripcion, T1.vp, T1.fortaleza, T1.oportunidad, T6.id, T6.nombre ");
            _query.Append("FROM Verificar T1 ");
            _query.Append("INNER JOIN Verificacion T2 ON T1.verificacionid = T2.id ");
            _query.Append("INNER JOIN Empleado T3 ON T1.empleadoid = T3.id ");
            _query.Append("INNER JOIN Estructura T4 ON T1.tableroid = T4.id ");
            _query.Append("INNER JOIN Estructura T5 ON T1.estructuraid = T5.id ");
            _query.Append("INNER JOIN TipoVerificacion T6 ON T2.tipoverificacionid = T6.id ");
            _query.Append("WHERE T1.tableroid = @tableroid ");
            _query.Append("AND  T1.fecharegistro BETWEEN @fechadesde AND @fechahasta ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("tableroid", tableroId);
                cmd.Parameters.AddWithValue("fechadesde", fechaDesde);
                cmd.Parameters.AddWithValue("fechahasta", fechaHasta.AddDays(1));

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.HasRows)
                        while (rd.Read())
                            lista.Add(new Verificar
                            {
                                Id = rd.GetInt32(0),
                                EmpleadoId = rd.GetInt32(1),
                                VerificacionId = rd.GetInt32(2),
                                Encargado = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                                FechaRegistro = rd.GetDateTime(4),
                                PuntajeMaximo = rd.GetInt32(5),
                                PuntajeObtenido = rd.GetInt32(6),
                                Verificacion = new Domain.Genericos.Verificacion.Verificacion
                                {
                                    Nombre = rd.GetString(7),
                                    TipoVerificacion = new TipoVerificacion
                                    {
                                        Id = rd.GetInt32(18),
                                        Nombre = rd.GetString(19)
                                    }
                                },
                                Empleado = new Empleado
                                {
                                    ApellidoPaterno = rd.GetString(8),
                                    ApellidoMaterno = rd.GetString(9),
                                    Nombre = rd.GetString(10)
                                },
                                TableroId = rd.GetInt32(11),
                                Tablero = new Estructura { Id = rd.GetInt32(11), Descripcion = rd.GetString(12) },
                                EstructuraId = rd.GetInt32(13),
                                Estructura = new Estructura { Id = rd.GetInt32(13), Descripcion = rd.GetString(14) },
                                VP = rd.GetBoolean(15),
                                Fortaleza = !rd.IsDBNull(16) ? rd.GetString(16) : null,
                                Oportunidad = !rd.IsDBNull(17) ? rd.GetString(17) : null
                            });

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar las listas de confirmaciones.", ex);
        }
    }

    public Verificar? Buscar(int id)
    {
        Verificar? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.empleadoid, T1.verificacionid, T1.encargado, T1.rom, T1.nrorom, T1.fortaleza, T1.oportunidad, T1.puntajemaximo, T1.puntajeobtenido, T1.fecharegistro, ");
            _query.Append(
                "T2.apellidopaterno, T2.apellidomaterno, T2.nombre, T3.nombre, T1.instructivoestandar, T1.tableroid, T4.descripcion, T1.estructuraid, T5.descripcion ");
            _query.Append("FROM Verificar T1 ");
            _query.Append("INNER JOIN Empleado T2 ON T1.empleadoid = T2.id ");
            _query.Append("INNER JOIN Verificacion T3 ON T1.verificacionid = T3.id ");
            _query.Append("INNER JOIN Estructura T4 ON T1.tableroid = T4.id ");
            _query.Append("INNER JOIN Estructura T5 ON T1.estructuraid = T5.id ");
            _query.Append("WHERE T1.id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Verificar
                        {
                            Id = rd.GetInt32(0),
                            EmpleadoId = rd.GetInt32(1),
                            VerificacionId = rd.GetInt32(2),
                            Encargado = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            Rom = !rd.IsDBNull(4) ? rd.GetBoolean(4) : (bool?)null,
                            NroRom = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                            Fortaleza = !rd.IsDBNull(6) ? rd.GetString(6) : null,
                            Oportunidad = !rd.IsDBNull(7) ? rd.GetString(7) : null,
                            PuntajeMaximo = rd.GetInt32(8),
                            PuntajeObtenido = rd.GetInt32(9),
                            FechaRegistro = rd.GetDateTime(10),
                            Empleado = new Empleado
                            {
                                ApellidoPaterno = rd.GetString(11),
                                ApellidoMaterno = rd.GetString(12),
                                Nombre = rd.GetString(13)
                            },
                            Verificacion = new Domain.Genericos.Verificacion.Verificacion { Nombre = rd.GetString(14) },
                            InstructivoEstandar = !rd.IsDBNull(15) ? rd.GetString(15) : null,
                            TableroId = rd.GetInt32(16),
                            Tablero = new Estructura { Descripcion = rd.GetString(17) },
                            EstructuraId = rd.GetInt32(18),
                            Estructura = new Estructura { Descripcion = rd.GetString(19) }
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar.", ex);
        }
    }

    public bool Guardar(Verificar entidad)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM usp_Verificar_Guardar(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15)";

                cmd.Parameters.AddWithValue("p0", entidad.EmpleadoId);
                cmd.Parameters.AddWithValue("p1", entidad.VerificacionId);
                cmd.Parameters.AddWithValue("p2", entidad.Encargado ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Rom ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.NroRom ?? _NullValue);
                cmd.Parameters.AddWithValue("p5", entidad.Fortaleza ?? _NullValue);
                cmd.Parameters.AddWithValue("p6", entidad.Oportunidad ?? _NullValue);
                cmd.Parameters.AddWithValue("p7", entidad.PuntajeMaximo);
                cmd.Parameters.AddWithValue("p8", entidad.PuntajeObtenido);
                cmd.Parameters.AddWithValue("p9", entidad.TableroId);
                cmd.Parameters.AddWithValue("p10", entidad.InstructivoEstandar ?? _NullValue);
                cmd.Parameters.AddWithValue("p11", entidad.UsuarioId);
                cmd.Parameters.AddWithValue("p12", entidad.VP);
                cmd.Parameters.AddWithValue("p13", entidad.AreaId ?? _NullValue);
                cmd.Parameters.AddWithValue("p14", entidad.EstructuraId);
                cmd.Parameters.AddWithValue("p15", entidad.IGP);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una verificación.", ex);
            //throw ex;
        }
    }

    #region Constructores

    public VerificarRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}