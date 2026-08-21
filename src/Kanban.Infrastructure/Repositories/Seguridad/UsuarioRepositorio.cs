using System.Data;
using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;
using NpgsqlTypes;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class UsuarioRepositorio : BaseRepositorio, IUsuarioRepositorio
{
    public int ContarUsuario(int id, string usuario)
    {
        var _parametros = new List<NpgsqlParameter>();
        var cantidad = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT COUNT(T1.*) ");
            _query.Append("FROM Usuario T1 ");
            _query.Append("WHERE T1.id <> @id ");
            _query.Append("AND T1.nombre = @nombre ");

            _parametros.Add(new NpgsqlParameter("id", id));
            _parametros.Add(new NpgsqlParameter("nombre", usuario));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                cantidad = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return cantidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al contar los usuarios", ex);
        }
    }

    public PagedResult<Usuario> ListarPorPagina(UsuarioFiltro? filter, int page, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Usuario>? lista = null;
        var totalRows = 0;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.nombre, T1.activo, T2.nombre, T2.apellidopaterno, T2.apellidomaterno, T3.nombre ");
            _query.Append("FROM Usuario T1 ");
            _query.Append("INNER JOIN Empleado T2 ON T1.empleadoid = T2.id ");
            _query.Append("INNER JOIN Rol T3 ON T1.rolid = T3.id ");

            if (filter != null)
            {
                _queryConditions.Append("WHERE (eliminado IS NULL OR eliminado = FALSE) ");

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

                if (!string.IsNullOrEmpty(filter.EmpleadoNombre))
                {
                    _queryConditions.Append("AND T2.nombre ILIKE '%' || @empleadonombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("empleadonombre", filter.EmpleadoNombre));
                }

                if (!string.IsNullOrEmpty(filter.EmpleadoApellidoPaterno))
                {
                    _queryConditions.Append("AND T2.apellidopaterno ILIKE '%' || @empleadoapellidopaterno || '%' ");
                    _parametros.Add(new NpgsqlParameter("empleadoapellidopaterno", filter.EmpleadoApellidoPaterno));
                }

                if (!string.IsNullOrEmpty(filter.EmpleadoApellidoMaterno))
                {
                    _queryConditions.Append("AND T2.apellidomaterno ILIKE '%' || @empleadoapellidomaterno || '%' ");
                    _parametros.Add(new NpgsqlParameter("empleadoapellidomaterno", filter.EmpleadoApellidoMaterno));
                }

                if (!string.IsNullOrEmpty(filter.RolNombre))
                {
                    _queryConditions.Append("AND T3.nombre ILIKE '%' || @rolnombre || '%' ");
                    _parametros.Add(new NpgsqlParameter("rolnombre", filter.RolNombre));
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
                        lista = new List<Usuario>();
                        while (rd.Read())
                            lista.Add(new Usuario
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Activo = rd.GetBoolean(2),
                                Empleado = new Empleado
                                {
                                    Nombre = rd.GetString(3),
                                    ApellidoPaterno = rd.GetString(4),
                                    ApellidoMaterno = rd.GetString(5)
                                },
                                Rol = new Rol { Nombre = rd.GetString(6) }
                            });
                    }

                    rd.Close();
                }

                _query = new StringBuilder();
                _query.Append("SELECT COUNT(T1.id) FROM Usuario T1 ");
                _query.Append("INNER JOIN Empleado T2 ON T1.empleadoid = T2.id ");
                _query.Append("INNER JOIN Rol T3 ON T1.rolid = T3.id ");

                if (filter != null) _query.Append(_queryConditions);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _query.ToString();

                totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return new PagedResult<Usuario>(lista ?? [], totalRows, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los usuarios.", ex);
        }
    }

    public Usuario? BuscarLogin(string nombre, string clave)
    {
        Usuario? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.empleadoid, T1.rolid, T1.activo, T2.nombre, T2.apellidopaterno, T2.apellidomaterno, T2.areaid, T3.descripcion, T4.nombre, T1.estructuraid, T5.descripcion, T1.cambioclave, T1.diasvencimiento ");
            _query.Append("FROM Usuario T1 ");
            _query.Append("INNER JOIN Empleado T2 ON T1.empleadoid = T2.id ");
            _query.Append("INNER JOIN Area T3 ON T2.areaid = T3.id ");
            _query.Append("INNER JOIN Rol T4 ON T1.rolid = T4.id ");
            _query.Append("INNER JOIN Estructura T5 ON T1.estructuraid = T5.id ");
            _query.Append("WHERE (T1.eliminado IS NULL OR T1.eliminado = FALSE) ");
            _query.Append("AND T1.nombre = @nombre ");
            _query.Append("AND  T1.clave = @clave ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("nombre", nombre);
                cmd.Parameters.AddWithValue("clave", clave);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        entidad = new Usuario
                        {
                            Id = rd.GetInt32(0),
                            Nombre = nombre,
                            EmpleadoId = rd.GetInt32(1),
                            RolId = rd.GetInt32(2),
                            Activo = rd.GetBoolean(3),
                            Empleado = new Empleado
                            {
                                Id = rd.GetInt32(1),
                                Nombre = rd.GetString(4),
                                ApellidoPaterno = rd.GetString(5),
                                ApellidoMaterno = rd.GetString(6),
                                AreaId = rd.GetInt32(7),
                                Area = new Area
                                {
                                    Id = rd.GetInt32(7),
                                    Descripcion = rd.GetString(8)
                                }
                            },
                            Rol = new Rol { Nombre = rd.GetString(9) },
                            EstructuraId = rd.GetInt32(10),
                            Estructura = new Estructura
                            {
                                Id = rd.GetInt32(10),
                                Descripcion = rd.GetString(11)
                            },
                            CambioClave = rd.GetFieldValue<DateTime?>(12),
                            DiasVencimiento = rd.GetFieldValue<int?>(13)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al buscar el usuario para el login.", ex);
        }
    }

    public Usuario? Buscar(int id)
    {
        Usuario? entidad = null;
        try
        {
            var _query = new StringBuilder();

            _query.Append(
                "SELECT T1.id, T1.empleadoid, T1.rolid, T1.activo, T2.nombre, T2.apellidopaterno, T2.apellidomaterno, T2.areaid, T3.descripcion, T4.nombre, T1.estructuraid, T5.descripcion, T1.cambioclave, T1.nombre, T1.diasvencimiento, T1.clave ");
            _query.Append("FROM Usuario T1 ");
            _query.Append("INNER JOIN Empleado T2 ON T1.empleadoid = T2.id ");
            _query.Append("INNER JOIN Area T3 ON T2.areaid = T3.id ");
            _query.Append("INNER JOIN Rol T4 ON T1.rolid = T4.id ");
            _query.Append("INNER JOIN Estructura T5 ON T1.estructuraid = T5.id ");
            _query.Append("WHERE (T1.eliminado IS NULL OR T1.eliminado = FALSE) ");
            _query.Append("AND T1.id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        entidad = new Usuario
                        {
                            Id = rd.GetInt32(0),
                            EmpleadoId = rd.GetInt32(1),
                            RolId = rd.GetInt32(2),
                            Activo = rd.GetBoolean(3),
                            Empleado = new Empleado
                            {
                                Id = rd.GetInt32(1),
                                Nombre = rd.GetString(4),
                                ApellidoPaterno = rd.GetString(5),
                                ApellidoMaterno = rd.GetString(6),
                                AreaId = rd.GetInt32(7),
                                Area = new Area
                                {
                                    Id = rd.GetInt32(7),
                                    Descripcion = rd.GetString(8)
                                }
                            },
                            Rol = new Rol { Nombre = rd.GetString(9) },
                            EstructuraId = rd.GetInt32(10),
                            Estructura = new Estructura
                            {
                                Id = rd.GetInt32(10),
                                Descripcion = rd.GetString(11)
                            },
                            CambioClave = rd.GetFieldValue<DateTime?>(12),
                            Nombre = rd.GetString(13),
                            DiasVencimiento = rd.GetFieldValue<int?>(14),
                            Clave = rd.GetFieldValue<string>(15)
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

    public void Guardar(Usuario entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Clave);
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Usuario_Guardar(@p0, @p1, @p2, @p3, @p4, @p5, @p6)";

                cmd.Parameters.AddWithValue("p0", entidad.RolId);
                cmd.Parameters.AddWithValue("p1", entidad.EmpleadoId);
                cmd.Parameters.AddWithValue("p2", entidad.EstructuraId);
                cmd.Parameters.AddWithValue("p3", entidad.Nombre);
                cmd.Parameters.AddWithValue("p4", entidad.Clave);
                cmd.Parameters.AddWithValue("p5", entidad.Activo);
                cmd.Parameters.AddWithValue("p6", entidad.DiasVencimiento ?? _NullValue);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el usuario", ex);
        }
    }

    public void Actualizar(Usuario entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Usuario_Actualizar(@p0, @p1, @p2, @p3, @p4, @p5, @p6)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.RolId);
                cmd.Parameters.AddWithValue("p2", entidad.EmpleadoId);
                cmd.Parameters.AddWithValue("p3", entidad.EstructuraId);
                cmd.Parameters.AddWithValue("p4", entidad.Nombre);
                cmd.Parameters.AddWithValue("p5", entidad.Activo);
                cmd.Parameters.AddWithValue("p6", entidad.DiasVencimiento ?? _NullValue);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al actualizar el usuario.", ex);
        }
    }

    public void CambiarClave(Usuario entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Clave);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Usuario_CambiarClave(@p0, @p1, @p2)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Clave);
                cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Date, entidad.CambioClave ?? _NullValue);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al actualizar la clave.", ex);
        }
    }

    public void Token(int id, string token, string ip)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Usuario_Token(@p0, @p1, @p2)";

                cmd.Parameters.AddWithValue("p0", id);
                cmd.Parameters.AddWithValue("p1", token);
                cmd.Parameters.AddWithValue("p2", ip);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el token.", ex);
        }
    }

    public bool ValidarToken(int id, string token, string ip)
    {
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT 1 ");
            _query.Append("FROM Usuario ");
            _query.Append("WHERE id = @id ");
            _query.Append("AND token = @token ");
            //_query.Append("AND ip = @ip ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("token", token);
                //cmd.Parameters.AddWithValue("ip", ip);

                var respuesta = false;
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) respuesta = rd.GetInt32(0) == 1;
                }

                return respuesta;
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al validar el token.", ex);
        }
    }

    #region Constructores

    public UsuarioRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}