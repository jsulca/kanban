using System.Text;
using Kanban.Application.Abstractions.Repositories.Compromiso;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Compromisos;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Compromisos;

public class CompromisoInstanciaRepositorio : BaseRepositorio, ICompromisoInstanciaRepositorio
{
    public List<CompromisoInstancia> Listar(int compromisoId)
    {
        List<CompromisoInstancia> lista = new();

        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.compromisoid, T1.instanciaid, T1.motivo, T1.fecharegistro, T2.abreviatura, T2.descripcion, T1.usuarioid, T1.empleadoid, T3.nombre, T4.nombre, T4.apellidopaterno, T4.apellidomaterno ");
            _query.Append("FROM CompromisoInstancia T1 ");
            _query.Append("INNER JOIN Instancia T2 ON T1.instanciaid = T2.id ");
            _query.Append("LEFT JOIN Usuario T3 ON T1.usuarioid = T3.id ");
            _query.Append("LEFT JOIN Empleado T4 ON T1.empleadoid = T4.id ");
            _query.Append("WHERE T1.compromisoid = @compromisoid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("compromisoid", compromisoId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new CompromisoInstancia
                        {
                            CompromisoId = rd.GetInt32(0),
                            InstanciaId = rd.GetInt32(1),
                            Motivo = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            FechaRegistro = rd.GetDateTime(3),
                            Instancia = new Instancia
                            {
                                Abreviatura = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                                Descripcion = rd.GetString(5)
                            },
                            UsuarioId = !rd.IsDBNull(6) ? rd.GetInt32(6) : (int?)null,
                            EmpleadoId = !rd.IsDBNull(7) ? rd.GetInt32(7) : (int?)null,
                            Usuario = rd.IsDBNull(6) ? null : new Usuario { Nombre = rd.GetString(8) },
                            Empleado = rd.IsDBNull(7)
                                ? null
                                : new Empleado
                                {
                                    Nombre = rd.GetString(9),
                                    ApellidoPaterno = rd.GetString(10),
                                    ApellidoMaterno = rd.GetString(11)
                                }
                        });

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de listar las instancias de un compromiso.", ex);
        }
    }

    public List<CompromisoInstancia> Exportar(int[] compromisosId)
    {
        List<CompromisoInstancia> lista = new();

        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.compromisoid, T1.instanciaid, T1.motivo, T1.fecharegistro, T2.abreviatura, T2.descripcion, T1.usuarioid, T1.empleadoid, T3.nombre, T4.nombre, T4.apellidopaterno, T4.apellidomaterno ");
            _query.Append("FROM CompromisoInstancia T1 ");
            _query.Append("INNER JOIN Instancia T2 ON T1.instanciaid = T2.id ");
            _query.Append("LEFT JOIN Usuario T3 ON T1.usuarioid = T3.id ");
            _query.Append("LEFT JOIN Empleado T4 ON T1.empleadoid = T4.id ");
            _query.Append("WHERE T1.compromisoid::TEXT IN (SELECT * FROM regexp_split_to_table(@ids, ',')) ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("ids", string.Join(",", compromisosId));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new CompromisoInstancia
                        {
                            CompromisoId = rd.GetInt32(0),
                            InstanciaId = rd.GetInt32(1),
                            Motivo = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            FechaRegistro = rd.GetDateTime(3),
                            Instancia = new Instancia
                            {
                                Abreviatura = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                                Descripcion = rd.GetString(5)
                            },
                            UsuarioId = !rd.IsDBNull(6) ? rd.GetInt32(6) : (int?)null,
                            EmpleadoId = !rd.IsDBNull(7) ? rd.GetInt32(7) : (int?)null,
                            Usuario = rd.IsDBNull(6) ? null : new Usuario { Nombre = rd.GetString(8) },
                            Empleado = rd.IsDBNull(7)
                                ? null
                                : new Empleado
                                {
                                    Nombre = rd.GetString(9),
                                    ApellidoPaterno = rd.GetString(10),
                                    ApellidoMaterno = rd.GetString(11)
                                }
                        });

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de listar las instancias de un compromiso.", ex);
        }
    }


    public bool Guardar(CompromisoInstancia entidad)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_CompromisoInstancia_Guardar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.CompromisoId);
                cmd.Parameters.AddWithValue("p1", entidad.InstanciaId);
                cmd.Parameters.AddWithValue("p2", entidad.Motivo ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.UsuarioId ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.EmpleadoId ?? _NullValue);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de guardar la instancia del compromiso.", ex);
        }
    }

    #region Constructores

    public CompromisoInstanciaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}