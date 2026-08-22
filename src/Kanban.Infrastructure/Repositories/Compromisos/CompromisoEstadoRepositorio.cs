using System.Text;
using Kanban.Application.Abstractions.Repositories.Compromiso;
using Kanban.Domain;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Compromisos;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Compromisos;

public class CompromisoEstadoRepositorio : BaseRepositorio, ICompromisoEstadoRepositorio
{
    public List<CompromisoEstado> Listar(int compromisoId)
    {
        List<CompromisoEstado> lista = new();

        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.compromisoid, T1.estado, T1.fecharegistro, T1.motivo, T1.usuarioid, T1.empleadoid, T2.nombre, T3.nombre, T3.apellidopaterno, T3.apellidomaterno ");
            _query.Append("FROM CompromisoEstado T1 ");
            _query.Append("LEFT JOIN Usuario T2 ON T1.usuarioid = T2.id ");
            _query.Append("LEFT JOIN Empleado T3 ON T1.empleadoid = T3.id ");
            _query.Append("WHERE compromisoid = @compromisoid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("compromisoid", compromisoId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new CompromisoEstado
                        {
                            CompromisoId = rd.GetInt32(0),
                            Estado = (EstadoCompromiso)rd.GetInt32(1),
                            FechaRegistro = rd.GetDateTime(2),
                            Motivo = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            UsuarioId = !rd.IsDBNull(4) ? rd.GetInt32(4) : (int?)null,
                            EmpleadoId = !rd.IsDBNull(5) ? rd.GetInt32(5) : (int?)null,
                            Usuario = rd.IsDBNull(4) ? null : new Usuario { Nombre = rd.GetString(6) },
                            Empleado = rd.IsDBNull(5)
                                ? null
                                : new Empleado
                                {
                                    Nombre = rd.GetString(7),
                                    ApellidoPaterno = rd.GetString(8),
                                    ApellidoMaterno = rd.GetString(9)
                                }
                        });

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de listar los estados de un compromiso.", ex);
        }
    }

    public List<CompromisoEstado> Exportar(int[] compromisosId)
    {
        List<CompromisoEstado> lista = new();

        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.compromisoid, T1.estado, T1.fecharegistro, T1.motivo, T1.usuarioid, T1.empleadoid, T2.nombre, T3.nombre, T3.apellidopaterno, T3.apellidomaterno ");
            _query.Append("FROM CompromisoEstado T1 ");
            _query.Append("LEFT JOIN Usuario T2 ON T1.usuarioid = T2.id ");
            _query.Append("LEFT JOIN Empleado T3 ON T1.empleadoid = T3.id ");
            _query.Append("WHERE T1.compromisoid::TEXT IN (SELECT * FROM regexp_split_to_table(@ids, ',')) ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("ids", string.Join(",", compromisosId));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new CompromisoEstado
                        {
                            CompromisoId = rd.GetInt32(0),
                            Estado = (EstadoCompromiso)rd.GetInt32(1),
                            FechaRegistro = rd.GetDateTime(2),
                            Motivo = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            UsuarioId = !rd.IsDBNull(4) ? rd.GetInt32(4) : (int?)null,
                            EmpleadoId = !rd.IsDBNull(5) ? rd.GetInt32(5) : (int?)null,
                            Usuario = rd.IsDBNull(4) ? null : new Usuario { Nombre = rd.GetString(6) },
                            Empleado = rd.IsDBNull(5)
                                ? null
                                : new Empleado
                                {
                                    Nombre = rd.GetString(7),
                                    ApellidoPaterno = rd.GetString(8),
                                    ApellidoMaterno = rd.GetString(9)
                                }
                        });

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de listar los estados de un compromiso.", ex);
        }
    }


    public bool Guardar(CompromisoEstado entidad)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_CompromisoEstado_Guardar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.CompromisoId);
                cmd.Parameters.AddWithValue("p1", (int)entidad.Estado);
                cmd.Parameters.AddWithValue("p2", entidad.Motivo ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.UsuarioId ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.EmpleadoId ?? _NullValue);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de guardar el estado del compromiso.", ex);
        }
    }

    #region Constructores

    public CompromisoEstadoRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}