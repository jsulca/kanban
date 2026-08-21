using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class EmpleadoRepositorio : BaseRepositorio, IEmpleadoRepositorio
{
    public List<Empleado> Listar(EmpleadoFiltro? filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<Empleado> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.cargoid, T1.areaid, T1.nombre, T1.apellidopaterno, T1.apellidomaterno, T1.nrodocumento, T1.correo, T1.telefono, ");
            _query.Append("T2.descripcion, T3.descripcion ");
            _query.Append("FROM Empleado T1 ");
            _query.Append("LEFT JOIN Cargo T2 ON T1.cargoid = T2.id ");
            _query.Append("INNER JOIN Area T3 ON T1.areaid = T3.id ");

            if (filtro != null)
            {
                _query.Append("WHERE 1 = 1 ");

                if (filtro.AreaId.HasValue)
                {
                    _queryConditions.Append("AND T1.areaid = @areaid ");
                    _parametros.Add(new NpgsqlParameter("areaid", filtro.AreaId.Value));
                }

                if (filtro.Ids != null && filtro.Ids.Length > 0)
                {
                    _queryConditions.Append("AND T1.id::TEXT IN (SELECT * FROM regexp_split_to_table(@ids, ',')) ");
                    _parametros.Add(new NpgsqlParameter("ids", string.Join(",", filtro.Ids)));
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
                        lista.Add(new Empleado
                        {
                            Id = rd.GetInt32(0),
                            CargoId = !rd.IsDBNull(1) ? rd.GetInt32(1) : (int?)null,
                            AreaId = rd.GetInt32(2),
                            Nombre = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            ApellidoPaterno = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                            ApellidoMaterno = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                            NroDocumento = !rd.IsDBNull(6) ? rd.GetString(6) : null,
                            Correo = !rd.IsDBNull(7) ? rd.GetString(7) : null,
                            Telefono = !rd.IsDBNull(8) ? rd.GetString(8) : null,
                            Cargo = rd.IsDBNull(1) ? null : new Cargo { Descripcion = rd.GetString(9) },
                            Area = rd.IsDBNull(2) ? null : new Area { Descripcion = rd.GetString(10) }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los empleados.", ex);
        }
    }

    public Empleado? Buscar(int id)
    {
        Empleado? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.id, T1.cargoid, T1.areaid, T1.nombre, T1.apellidopaterno, T1.apellidomaterno, T1.nrodocumento, T1.correo, T1.telefono, ");
            _query.Append("T2.descripcion, T3.descripcion ");
            _query.Append("FROM Empleado T1 ");
            _query.Append("LEFT JOIN Cargo T2 ON T1.cargoid = T2.id ");
            _query.Append("INNER JOIN Area T3 ON T1.areaid = T3.id ");
            _query.Append("WHERE T1.id = @id");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Empleado
                        {
                            Id = rd.GetInt32(0),
                            CargoId = !rd.IsDBNull(1) ? rd.GetInt32(1) : (int?)null,
                            AreaId = rd.GetInt32(2),
                            Nombre = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            ApellidoPaterno = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                            ApellidoMaterno = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                            NroDocumento = !rd.IsDBNull(6) ? rd.GetString(6) : null,
                            Correo = !rd.IsDBNull(7) ? rd.GetString(7) : null,
                            Telefono = !rd.IsDBNull(8) ? rd.GetString(8) : null
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar un empleado.", ex);
        }
    }

    public bool Guardar(Empleado entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.ApellidoMaterno);
        ArgumentNullException.ThrowIfNull(entidad.ApellidoPaterno);
        ArgumentNullException.ThrowIfNull(entidad.Nombre);
        ArgumentNullException.ThrowIfNull(entidad.NroDocumento);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Empleado_Guardar(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)";

                cmd.Parameters.AddWithValue("p0", entidad.CargoId ?? _NullValue);
                cmd.Parameters.AddWithValue("p1", entidad.AreaId);
                cmd.Parameters.AddWithValue("p2", entidad.Nombre.ToUpper());
                cmd.Parameters.AddWithValue("p3", entidad.ApellidoPaterno.ToUpper());
                cmd.Parameters.AddWithValue("p4", entidad.ApellidoMaterno.ToUpper());
                cmd.Parameters.AddWithValue("p5", entidad.NroDocumento.ToUpper());
                cmd.Parameters.AddWithValue("p6", entidad.Correo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p7", entidad.Telefono?.ToUpper() ?? _NullValue);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar un empleado.", ex);
        }
    }

    public void Guardar(List<Empleado> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Empleado_Guardar(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p3" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p5" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p6" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p7" });

                foreach (var entidad in entidades)
                {
                    ArgumentNullException.ThrowIfNull(entidad.ApellidoMaterno);
                    ArgumentNullException.ThrowIfNull(entidad.ApellidoPaterno);
                    ArgumentNullException.ThrowIfNull(entidad.Nombre);
                    ArgumentNullException.ThrowIfNull(entidad.NroDocumento);

                    cmd.Parameters["p0"].Value = entidad.CargoId ?? _NullValue;
                    cmd.Parameters["p1"].Value = entidad.AreaId;
                    cmd.Parameters["p2"].Value = entidad.Nombre.ToUpper();
                    cmd.Parameters["p3"].Value = entidad.ApellidoPaterno.ToUpper();
                    cmd.Parameters["p4"].Value = entidad.ApellidoMaterno.ToUpper();
                    cmd.Parameters["p5"].Value = entidad.NroDocumento.ToUpper();
                    cmd.Parameters["p6"].Value = entidad.Correo?.ToUpper() ?? _NullValue;
                    cmd.Parameters["p7"].Value = entidad.Telefono?.ToUpper() ?? _NullValue;

                    entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar varios empleados.", ex);
        }
    }

    public bool Actualizar(Empleado entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.ApellidoMaterno);
        ArgumentNullException.ThrowIfNull(entidad.ApellidoPaterno);
        ArgumentNullException.ThrowIfNull(entidad.Nombre);
        ArgumentNullException.ThrowIfNull(entidad.NroDocumento);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Empleado_Actualizar(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.CargoId ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.AreaId);
                cmd.Parameters.AddWithValue("p3", entidad.Nombre.ToUpper());
                cmd.Parameters.AddWithValue("p4", entidad.ApellidoPaterno.ToUpper());
                cmd.Parameters.AddWithValue("p5", entidad.ApellidoMaterno.ToUpper());
                cmd.Parameters.AddWithValue("p6", entidad.NroDocumento.ToUpper());
                cmd.Parameters.AddWithValue("p7", entidad.Correo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p8", entidad.Telefono?.ToUpper() ?? _NullValue);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar un empleado.", ex);
        }
    }

    #region Constructores

    public EmpleadoRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}