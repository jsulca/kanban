using System.Data;
using System.Text;
using Kanban.Application.Abstractions.Repositories.Compromiso;
using Kanban.Application.Common;
using Kanban.Domain;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Compromisos;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;
using NpgsqlTypes;

namespace Kanban.Infrastructure.Repositories.Compromisos;

public class CompromisoRepositorio : BaseRepositorio, ICompromisoRepositorio
{
    public PagedResult<Compromiso> ListarPorPagina(CompromisoFiltro? filtro, int page, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Compromiso> lista = new();
        var totalRows = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.codigo, T1.descripcion, T1.fecharegistro, T1.estado, T2.descripcion, T1.areaid, T4.clase, T4.rgba, T4.hex, T5.clase, T5.rgba, T5.hex, T1.instanciaid, T6.abreviatura, ");
            _query.Append("T1.fechaprogramacion, T1.fechareprogramacion, T3.descripcion, T7.descripcion ");
            _query.Append("FROM compromiso T1 ");
            _query.Append("INNER JOIN estructura T2 ON T1.tableroid = T2.id ");
            _query.Append("LEFT JOIN area T3 ON T1.areaid = T3.id ");
            _query.Append("LEFT JOIN color T4 ON T3.colorfondoid = T4.id ");
            _query.Append("LEFT JOIN color T5 ON T3.colortextoid = T5.id ");
            _query.Append("LEFT JOIN instancia T6 ON T1.instanciaid = T6.id ");
            _query.Append("INNER JOIN estructura T7 ON T1.estructuraid = T7.id ");

            if (filtro != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (filtro.TableroId.HasValue)
                {
                    _queryConditions.Append("AND T1.tableroid = @tableroid ");
                    _parametros.Add(new NpgsqlParameter("tableroid", filtro.TableroId.Value));
                }

                if (!string.IsNullOrEmpty(filtro.Codigo))
                {
                    _queryConditions.Append("AND T1.codigo ILIKE '%' || @codigo || '%' ");
                    _parametros.Add(new NpgsqlParameter("codigo", filtro.Codigo));
                }

                if (!string.IsNullOrEmpty(filtro.EstructuraDescripcion))
                {
                    _queryConditions.Append("AND T7.descripcion ILIKE '%' || @estructuradescripcion || '%' ");
                    _parametros.Add(new NpgsqlParameter("estructuradescripcion", filtro.EstructuraDescripcion));
                }

                if (!string.IsNullOrEmpty(filtro.Descripcion))
                {
                    _queryConditions.Append("AND T1.descripcion ILIKE '%' || @descripcion || '%' ");
                    _parametros.Add(new NpgsqlParameter("descripcion", filtro.Descripcion));
                }

                if (!string.IsNullOrEmpty(filtro.FechaRegistroDesde) &&
                    DateTime.TryParse(filtro.FechaRegistroDesde, out DateTime fechaRegistroDesde))
                {
                    _queryConditions.Append("AND T1.fecharegistro >= @fecharegistrodesde ");
                    _parametros.Add(new NpgsqlParameter("fecharegistrodesde", fechaRegistroDesde));
                }

                if (!string.IsNullOrEmpty(filtro.FechaRegistroHasta) &&
                    DateTime.TryParse(filtro.FechaRegistroHasta, out DateTime fechaRegistroHasta))
                {
                    _queryConditions.Append("AND T1.fecharegistro <= @fecharegistrohasta ");
                    _parametros.Add(new NpgsqlParameter("fecharegistrohasta", fechaRegistroHasta.AddDays(1)));
                }

                if (filtro.Estado.HasValue)
                {
                    _queryConditions.Append("AND T1.estado = @estado ");
                    _parametros.Add(new NpgsqlParameter("estado", (int)filtro.Estado.Value));
                }

                if (filtro.Estados != null && filtro.Estados.Length > 0)
                {
                    _queryConditions.Append(
                        "AND T1.estado::TEXT IN (SELECT * FROM regexp_split_to_table(@estados, ',')) ");
                    _parametros.Add(new NpgsqlParameter("estados", string.Join(",", filtro.Estados)));
                }

                if (filtro.InstanciaId.HasValue)
                {
                    _queryConditions.Append("AND COALESCE(T1.instanciaid, 0) = @instanciaid ");
                    _parametros.Add(new NpgsqlParameter("instanciaid", filtro.InstanciaId.Value));
                }

                if (filtro.Instancias != null && filtro.Instancias.Length > 0)
                {
                    _queryConditions.Append(
                        "AND T1.instanciaid::TEXT IN (SELECT * FROM regexp_split_to_table(@instancias, ',')) ");
                    _parametros.Add(new NpgsqlParameter("instancias", string.Join(",", filtro.Instancias)));
                }

                if (filtro.ResponsableId.HasValue)
                {
                    _queryConditions.Append("AND T1.responsableid = @responsableid ");
                    _parametros.Add(new NpgsqlParameter("responsableid", filtro.ResponsableId.Value));
                }

                if (filtro.Estructuras != null && filtro.Estructuras.Length > 0)
                {
                    _queryConditions.Append(
                        "AND (T1.estructuraid::TEXT IN (SELECT * FROM regexp_split_to_table(@estructuras, ',')) OR T1.tableroid::TEXT IN (SELECT * FROM regexp_split_to_table(@estructuras, ','))) ");
                    _parametros.Add(new NpgsqlParameter("estructuras", string.Join(",", filtro.Estructuras)));
                }

                _query.Append(_queryConditions);
            }

            _query.Append("ORDER BY T1.id DESC ");
            _query.Append("LIMIT @desde ");
            _query.Append("OFFSET @hasta ");

            _parametros.Add(new NpgsqlParameter("desde", pageSize));
            _parametros.Add(new NpgsqlParameter("hasta", pageSize * (page - 1)));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Compromiso
                        {
                            Id = rd.GetInt32(0),
                            Codigo = rd.GetString(1),
                            Descripcion = rd.GetString(2),
                            FechaRegistro = rd.GetDateTime(3),
                            Estado = (EstadoCompromiso)rd.GetInt32(4),
                            Tablero = new Estructura { Descripcion = rd.GetString(5) },
                            AreaId = rd.GetFieldValue<int?>(6),
                            Area = rd.IsDBNull(6)
                                ? null
                                : new Area
                                {
                                    ColorFondo = new Color
                                    {
                                        Clase = !rd.IsDBNull(7) ? rd.GetString(7) : null,
                                        Rgba = !rd.IsDBNull(8) ? rd.GetString(8) : null,
                                        Hex = !rd.IsDBNull(9) ? rd.GetString(9) : null
                                    },
                                    ColorTexto = new Color
                                    {
                                        Clase = !rd.IsDBNull(10) ? rd.GetString(10) : null,
                                        Rgba = !rd.IsDBNull(11) ? rd.GetString(11) : null,
                                        Hex = !rd.IsDBNull(12) ? rd.GetString(12) : null
                                    },
                                    Descripcion = rd.GetString(17)
                                },
                            InstanciaId = rd.GetFieldValue<int?>(13),
                            Instancia = rd.IsDBNull(13)
                                ? null
                                : new Instancia { Abreviatura = !rd.IsDBNull(14) ? rd.GetString(14) : null },
                            FechaProgramacion = rd.GetFieldValue<DateTime?>(15),
                            FechaReprogramacion = rd.GetFieldValue<DateTime?>(16),
                            Estructura = new Estructura { Descripcion = rd.GetString(18) }
                        });
                    rd.Close();
                }

                _query = new StringBuilder();
                _query.Append("SELECT COUNT(T1.id) ");
                _query.Append("FROM compromiso T1 ");
                _query.Append("INNER JOIN estructura T2 ON T1.tableroid = T2.id ");
                _query.Append("LEFT JOIN area T3 ON T1.areaid = T3.id ");
                _query.Append("LEFT JOIN color T4 ON T3.colorfondoid = T4.id ");
                _query.Append("LEFT JOIN color T5 ON T3.colortextoid = T5.id ");
                _query.Append("LEFT JOIN instancia T6 ON T1.instanciaid = T6.id ");
                _query.Append("INNER JOIN estructura T7 ON T1.estructuraid = T7.id ");

                if (filtro != null) _query.Append(_queryConditions);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _query.ToString();

                totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return new PagedResult<Compromiso>(lista, totalRows, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de listar los compromisos por pagina.", ex);
        }
    }

    public List<Compromiso> Listar(CompromisoFiltro? filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Compromiso>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.codigo, T1.descripcion, T1.fecharegistro, T1.estado, T1.fechaprogramacion, T1.fechareprogramacion, T2.id, T3.id, T3.hex, T3.rgba, T3.clase, T1.instanciaid ");
            _query.Append("FROM Compromiso T1 ");
            _query.Append("LEFT JOIN Area T2 ON T1.areaid = T2.id ");
            _query.Append("LEFT JOIN Color T3 ON T2.colorfondoid = T3.id ");
            if (filtro != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                if (filtro.TableroId.HasValue)
                {
                    _queryConditions.Append("AND T1.tableroid = @tableroid ");
                    _parametros.Add(new NpgsqlParameter("tableroid", filtro.TableroId.Value));
                }

                if (filtro.Estado.HasValue)
                {
                    _queryConditions.Append("AND T1.estado = @estado ");
                    _parametros.Add(new NpgsqlParameter("estado", (int)filtro.Estado.Value));
                }

                if (filtro.InstanciaId.HasValue)
                {
                    _queryConditions.Append("AND COALESCE(T1.instanciaid, 0) = @instanciaid ");
                    _parametros.Add(new NpgsqlParameter("instanciaid", filtro.InstanciaId.Value));
                }

                if (filtro.Estados != null && filtro.Estados.Length > 0 && filtro.Instancias != null &&
                    filtro.Instancias.Length > 0)
                {
                    _queryConditions.Append(
                        "AND (T1.estado::TEXT IN (SELECT * FROM regexp_split_to_table(@estados, ',')) OR T1.instanciaid::TEXT IN (SELECT * FROM regexp_split_to_table(@instancias, ','))) ");
                    _parametros.Add(new NpgsqlParameter("estados", string.Join(",", filtro.Estados)));
                    _parametros.Add(new NpgsqlParameter("instancias", string.Join(",", filtro.Instancias)));
                }
                else
                {
                    if (filtro.Estados != null && filtro.Estados.Length > 0)
                    {
                        _queryConditions.Append(
                            "AND T1.estado::TEXT IN (SELECT * FROM regexp_split_to_table(@estados, ',')) ");
                        _parametros.Add(new NpgsqlParameter("estados", string.Join(",", filtro.Estados)));
                    }

                    if (filtro.Instancias != null && filtro.Instancias.Length > 0)
                    {
                        _queryConditions.Append(
                            "AND T1.instanciaid::TEXT IN (SELECT * FROM regexp_split_to_table(@instancias, ',')) ");
                        _parametros.Add(new NpgsqlParameter("instancias", string.Join(",", filtro.Instancias)));
                    }
                }

                _query.Append(_queryConditions);
            }

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<Compromiso>();

                    while (rd.Read())
                        lista.Add(new Compromiso
                        {
                            Id = rd.GetInt32(0),
                            Codigo = rd.GetString(1),
                            Descripcion = rd.GetString(2),
                            FechaRegistro = rd.GetDateTime(3),
                            Estado = (EstadoCompromiso)rd.GetInt32(4),
                            FechaProgramacion = !rd.IsDBNull(5) ? rd.GetDateTime(5) : (DateTime?)null,
                            FechaReprogramacion = !rd.IsDBNull(6) ? rd.GetDateTime(6) : (DateTime?)null,
                            AreaId = !rd.IsDBNull(7) ? rd.GetInt32(7) : (int?)null,
                            Area = rd.IsDBNull(7)
                                ? null
                                : new Area
                                {
                                    Id = rd.GetInt32(7),
                                    ColorFondo = rd.IsDBNull(8)
                                        ? null
                                        : new Color
                                        {
                                            Id = rd.GetInt32(8),
                                            Hex = !rd.IsDBNull(9) ? rd.GetString(9) : null,
                                            Rgba = !rd.IsDBNull(10) ? rd.GetString(10) : null,
                                            Clase = !rd.IsDBNull(11) ? rd.GetString(11) : null
                                        }
                                },
                            InstanciaId = !rd.IsDBNull(12) ? rd.GetInt32(12) : (int?)null
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de listar los compromisos.", ex);
        }
    }

    public Compromiso? Buscar(int id)
    {
        Compromiso? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.codigo, T1.descripcion, T1.impacto, T1.estado, T1.estructuraid, T1.detalle, T1.tableroid, T1.respuesta, T1.fecharegistro, T1.areaid, T1.responsableid, T1.instanciaid, T1.fechaprogramacion, ");
            _query.Append(
                "T1.fechareprogramacion, T1.accion, T1.fotoid, T1.origen, T1.usuarioregistroid, T2.nombre, T1.empleadoregistroid, T3.nombre, T3.apellidopaterno, T3.apellidomaterno, T4.descripcion, T5.descripcion, ");
            _query.Append("T1.planaccionid, T6.nombre, T6.ruta ");
            _query.Append("FROM Compromiso T1 ");
            _query.Append("INNER JOIN Usuario T2 ON T1.usuarioregistroid = T2.id ");
            _query.Append("INNER JOIN Empleado T3 ON T1.empleadoregistroid = T3.id ");
            _query.Append("INNER JOIN Estructura T4 ON T1.tableroid = T4.id ");
            _query.Append("INNER JOIN Estructura T5 ON T1.estructuraid = T5.id ");
            _query.Append("LEFT JOIN Adjunto T6 ON T1.fotoid = T6.id ");
            _query.Append("WHERE T1.id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Compromiso
                        {
                            Id = id,
                            Codigo = rd.GetString(0),
                            Descripcion = rd.GetString(1),
                            Impacto = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            Estado = (EstadoCompromiso)rd.GetInt32(3),
                            EstructuraId = rd.GetInt32(4),
                            Detalle = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                            TableroId = rd.GetInt32(6),
                            Respuesta = !rd.IsDBNull(7) ? rd.GetString(7) : null,
                            FechaRegistro = rd.GetDateTime(8),
                            AreaId = !rd.IsDBNull(9) ? rd.GetInt32(9) : (int?)null,
                            ResponsableId = !rd.IsDBNull(10) ? rd.GetInt32(10) : (int?)null,
                            InstanciaId = !rd.IsDBNull(11) ? rd.GetInt32(11) : (int?)null,
                            FechaProgramacion = !rd.IsDBNull(12) ? rd.GetDateTime(12) : (DateTime?)null,
                            FechaReprogramacion = !rd.IsDBNull(13) ? rd.GetDateTime(13) : (DateTime?)null,
                            Accion = !rd.IsDBNull(14) ? rd.GetString(14) : null,
                            FotoId = !rd.IsDBNull(15) ? rd.GetInt32(15) : (int?)null,
                            Origen = !rd.IsDBNull(16) ? rd.GetString(16) : null,
                            UsuarioRegistroId = rd.GetInt32(17),
                            UsuarioRegistro = new Usuario { Nombre = rd.GetString(18) },
                            EmpleadoRegistroId = rd.GetInt32(19),
                            EmpleadoRegistro = new Empleado
                            {
                                Nombre = rd.GetString(20),
                                ApellidoPaterno = rd.GetString(21),
                                ApellidoMaterno = rd.GetString(22)
                            },
                            Tablero = new Estructura { Descripcion = rd.GetString(23) },
                            Estructura = new Estructura { Descripcion = rd.GetString(24) },
                            PlanAccionId = !rd.IsDBNull(25) ? rd.GetInt32(25) : (int?)null,
                            Foto = rd.IsDBNull(15)
                                ? null
                                : new Adjunto
                                    { Nombre = rd.GetFieldValue<string>(26), Ruta = rd.GetFieldValue<string>(27) }
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de buscar el compromiso.", ex);
        }
    }

    public List<Compromiso> Exportar(CompromisoFiltro filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        if (filtro == null) throw new ArgumentNullException(nameof(filtro));
        // la consulta filtra siempre por tablero, no es opcional aqui
        ArgumentNullException.ThrowIfNull(filtro.TableroId);
        List<Compromiso> lista = new();

        var _query = new StringBuilder();
        try
        {
            _query.Append(
                "SELECT T1.id, T2.nombre, T3.descripcion, T4.descripcion, T1.descripcion, T1.fecharegistro, ");
            _query.Append("T1.origen, T1.impacto, T1.detalle, T1.estado, T5.descripcion, ");
            _query.Append(
                "T1.fechaprogramacion, T1.fechareprogramacion, T6.nombre, T6.apellidopaterno, T6.apellidomaterno, ");
            _query.Append(
                "T7.descripcion, T8.nombre, T8.apellidopaterno, T8.apellidomaterno, T1.accion, T1.fotoid, T1.respuesta ");
            _query.Append("FROM compromiso T1 ");
            _query.Append("INNER JOIN usuario T2 ON T1.usuarioregistroid = T2.id ");
            _query.Append("INNER JOIN estructura T3 ON T1.estructuraid = T3.id ");
            _query.Append("INNER JOIN estructura T4 ON T1.tableroid = T4.id ");
            _query.Append("LEFT JOIN instancia T5 ON T1.instanciaid = T5.id ");
            _query.Append("INNER JOIN empleado T6 ON T1.empleadoregistroid = T6.id ");

            _query.Append("LEFT JOIN area T7 ON T1.areaid = T7.id ");
            _query.Append("LEFT JOIN empleado T8 ON T1.responsableid = T8.id ");

            _query.Append("WHERE T1.tableroid = @tableroid ");
            _parametros.Add(new NpgsqlParameter("tableroid", filtro.TableroId.Value));

            DateTime.TryParse(filtro.FechaRegistroDesde, out DateTime fechaRegistroDesde);

            _query.Append("AND T1.fecharegistro >= @fecharegistrodesde ");
            _parametros.Add(new NpgsqlParameter("fecharegistrodesde", fechaRegistroDesde));

            DateTime.TryParse(filtro.FechaRegistroHasta, out DateTime fechaRegistroHasta);

            _query.Append("AND T1.fecharegistro <= @fecharegistrohasta ");
            _parametros.Add(new NpgsqlParameter("fecharegistrohasta", fechaRegistroHasta.AddDays(1)));


            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Compromiso
                        {
                            Id = rd.GetInt32(0),
                            UsuarioRegistro = new Usuario { Nombre = rd.GetFieldValue<string>(1) },
                            Estructura = new Estructura { Descripcion = rd.GetFieldValue<string>(2) },
                            Tablero = new Estructura { Descripcion = rd.GetFieldValue<string>(3) },
                            Descripcion = rd.GetFieldValue<string>(4),
                            FechaRegistro = rd.GetFieldValue<DateTime>(5),
                            Origen = rd.GetFieldValue<string>(6),
                            Impacto = rd.GetFieldValue<string>(7),
                            Detalle = rd.IsDBNull(8) ? null : rd.GetFieldValue<string>(8),
                            Estado = (EstadoCompromiso)rd.GetFieldValue<int>(9),
                            Instancia = rd.IsDBNull(10)
                                ? null
                                : new Instancia { Descripcion = rd.GetFieldValue<string>(10) },
                            FechaProgramacion = rd.GetFieldValue<DateTime?>(11),
                            FechaReprogramacion = rd.GetFieldValue<DateTime?>(12),
                            EmpleadoRegistro = new Empleado
                            {
                                Nombre = rd.IsDBNull(13) ? null : rd.GetFieldValue<string>(13),
                                ApellidoPaterno = rd.IsDBNull(14) ? null : rd.GetFieldValue<string>(14),
                                ApellidoMaterno = rd.IsDBNull(15) ? null : rd.GetFieldValue<string>(15)
                            },
                            Area = rd.IsDBNull(16) ? null : new Area { Descripcion = rd.GetFieldValue<string>(16) },
                            Responsable = rd.IsDBNull(17)
                                ? null
                                : new Empleado
                                {
                                    Nombre = rd.GetFieldValue<string>(17),
                                    ApellidoPaterno = rd.GetFieldValue<string>(18),
                                    ApellidoMaterno = rd.GetFieldValue<string>(19)
                                },
                            Accion = rd.IsDBNull(20) ? null : rd.GetFieldValue<string>(20),
                            FotoId = rd.GetFieldValue<int?>(21),
                            Respuesta = rd.IsDBNull(22) ? null : rd.GetFieldValue<string>(22)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los compromisos", ex);
        }
    }

    #region Constructores

    public CompromisoRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion

    #region Transacciones

    public bool Guardar(Compromiso entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Codigo);
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM usp_Compromiso_Guardar(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)";

                cmd.Parameters.AddWithValue("p0", entidad.Codigo);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion);
                cmd.Parameters.AddWithValue("p2", entidad.Detalle ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Impacto ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", (int)entidad.Estado);
                cmd.Parameters.AddWithValue("p5", entidad.EstructuraId);
                cmd.Parameters.AddWithValue("p6", entidad.TableroId);
                cmd.Parameters.AddWithValue("p7", entidad.FotoId ?? _NullValue);
                cmd.Parameters.AddWithValue("p8", entidad.Origen ?? _NullValue);
                cmd.Parameters.AddWithValue("p9", entidad.UsuarioRegistroId);
                cmd.Parameters.AddWithValue("p10", entidad.EmpleadoRegistroId);
                cmd.Parameters.AddWithValue("p11", entidad.PlanAccionId ?? _NullValue);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());

                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de guardar el compromiso.", ex);
        }
    }

    public bool Actualizar(Compromiso entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Compromiso_Actualizar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion);
                cmd.Parameters.AddWithValue("p2", entidad.Detalle ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Impacto ?? _NullValue);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de actualizar el compromiso.", ex);
        }
    }

    public int Contar(int estructuraId)
    {
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT COUNT(id) ");
            _query.Append("FROM Compromiso ");
            _query.Append("WHERE tableroid = @estructuraid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("estructuraid", estructuraId);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de contar los compromisos.", ex);
        }
    }

    public bool CambiarEstado(Compromiso entidad)
    {
        //int id, string respuesta, EstadoCompromiso estado
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Compromiso_CambiarEstado(@p0, @p1, @p2, @p3, @p4, @p5)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", (int)entidad.Estado);
                cmd.Parameters.AddWithValue("p2", entidad.Respuesta ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", NpgsqlDbType.Date, entidad.FechaProgramacion ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", NpgsqlDbType.Date, entidad.FechaReprogramacion ?? _NullValue);
                cmd.Parameters.AddWithValue("p5", entidad.Accion ?? _NullValue);

                cmd.ExecuteNonQuery();
                return true;
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de cambiar el estado del compromiso.", ex);
        }
    }

    public bool CambiarInstancia(int id, int? instanciaId)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Compromiso_CambiarInstancia(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", id);
                cmd.Parameters.AddWithValue("p1", instanciaId ?? _NullValue);

                cmd.ExecuteNonQuery();
                return true;
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de cambiar la instancia del compromiso.", ex);
        }
    }

    public bool Asignar(Compromiso entidad)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Compromiso_Asignar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.AreaId ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.ResponsableId ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Accion ?? _NullValue);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de asignar responsables al compromiso.", ex);
        }
    }

    public bool ReiniciarFecha(int id)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Compromiso_ReiniciarFecha(@p0)";
                cmd.Parameters.AddWithValue("p0", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de reiniciar las fechas.", ex);
        }
    }

    public void FueraFecha()
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Compromiso_FueraFecha()";
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de actualizar los compromisos a fuera de fecha.", ex);
        }
    }

    #endregion

    #region Indicadores

    public List<Compromiso> IndicadorPorEstado_1_1(int tableroId, DateTime fechaHasta)
    {
        List<Compromiso> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, estado, impacto, areaid, responsableid ");
            _query.Append("FROM Compromiso ");
            _query.Append("WHERE tableroid = @tableroid ");
            _query.Append("AND fecharegistro <= @fechahasta ");
            _query.Append("AND estado != 8 ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("tableroid", tableroId);
                cmd.Parameters.AddWithValue("fechahasta", fechaHasta.AddDays(1));

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<Compromiso>();

                    while (rd.Read())
                        lista.Add(new Compromiso
                        {
                            Id = rd.GetInt32(0),
                            Estado = (EstadoCompromiso)rd.GetInt32(1),
                            Impacto = rd.GetString(2),
                            AreaId = !rd.IsDBNull(3) ? rd.GetInt32(3) : (int?)null,
                            ResponsableId = !rd.IsDBNull(4) ? rd.GetInt32(4) : (int?)null
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de listar los compromisos para el indicador por estado 1 - 1.", ex);
        }
    }

    public List<Compromiso> IndicadorPorEstado_1_2(int tableroId, DateTime fechaDesde, DateTime fechaHasta)
    {
        List<Compromiso> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, estado, impacto, areaid, responsableid ");
            _query.Append("FROM Compromiso ");
            _query.Append("WHERE tableroid = @tableroid ");
            _query.Append("AND  fecharegistro BETWEEN @fechadesde AND @fechahasta ");
            _query.Append("AND  estado = @estado ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("tableroid", tableroId);
                cmd.Parameters.AddWithValue("fechaDesde", fechaDesde);
                cmd.Parameters.AddWithValue("fechahasta", fechaHasta.AddDays(1));
                cmd.Parameters.AddWithValue("estado", (int)EstadoCompromiso.FINALIZADO);

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<Compromiso>();

                    while (rd.Read())
                        lista.Add(new Compromiso
                        {
                            Id = rd.GetInt32(0),
                            Estado = (EstadoCompromiso)rd.GetInt32(1),
                            Impacto = rd.GetString(2),
                            AreaId = !rd.IsDBNull(3) ? rd.GetInt32(3) : (int?)null,
                            ResponsableId = !rd.IsDBNull(4) ? rd.GetInt32(4) : (int?)null
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de listar los compromisos para el indicador por estado 1 - 2.", ex);
        }
    }

    public List<Compromiso> PorTablero(int tableroId)
    {
        List<Compromiso> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, estado, empleadoregistroid, instanciaid ");
            _query.Append("FROM Compromiso ");
            _query.Append("WHERE tableroid = @tableroid ");
            _query.Append("AND estado != @estado ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("tableroid", tableroId);
                cmd.Parameters.AddWithValue("estado", (int)EstadoCompromiso.RECHAZADO);

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<Compromiso>();

                    while (rd.Read())
                        lista.Add(new Compromiso
                        {
                            Id = rd.GetInt32(0),
                            Estado = (EstadoCompromiso)rd.GetInt32(1),
                            EmpleadoRegistroId = rd.GetInt32(2),
                            InstanciaId = !rd.IsDBNull(3) ? rd.GetInt32(3) : (int?)null
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de listar los compromisos por tablero.", ex);
        }
    }

    #endregion
}