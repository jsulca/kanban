using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class EstructuraEmpleadoRepositorio : BaseRepositorio, IEstructuraEmpleadoRepositorio
{
    public List<EstructuraEmpleado> Listar(int estructuraId)
    {
        List<EstructuraEmpleado> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT	T1.estructuraid, T1.empleadoid, T1.areaid, T2.codigo, T2.descripcion, T3.nombre, T3.apellidopaterno, T3.apellidomaterno, T4.descripcion ");
            _query.Append("FROM EstructuraEmpleado T1 ");
            _query.Append("INNER JOIN Estructura T2 ON T1.estructuraid = T2.id ");
            _query.Append("INNER JOIN Empleado T3 ON T1.empleadoid = T3.id ");
            _query.Append("INNER JOIN Area T4 ON T1.areaid = T4.id ");
            _query.Append("WHERE T1.estructuraid = @estructuraid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("estructuraid", estructuraId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new EstructuraEmpleado
                        {
                            EstructuraId = rd.GetInt32(0),
                            EmpleadoId = rd.GetInt32(1),
                            AreaId = rd.GetInt32(2),
                            Estructura = new Estructura
                            {
                                Id = rd.GetInt32(0),
                                Codigo = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                                Descripcion = rd.GetString(4)
                            },
                            Empleado = new Empleado
                            {
                                Id = rd.GetInt32(1),
                                Nombre = rd.GetString(5),
                                ApellidoPaterno = rd.GetString(6),
                                ApellidoMaterno = rd.GetString(7)
                            },
                            Area = new Area
                            {
                                Id = rd.GetInt32(2),
                                Descripcion = rd.GetString(8)
                            }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los empleados de una estructura.", ex);
        }
    }

    public void Guardar(EstructuraEmpleado entidad)
    {
        try
        {
            Guardar(new List<EstructuraEmpleado> { entidad });
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar un empleado con una área.", ex);
        }
    }

    public void Guardar(List<EstructuraEmpleado> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_EstructuraEmpleado_Guardar(@p0, @p1, @p2)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2" });

                foreach (var entidad in entidades)
                {
                    cmd.Parameters["p0"].Value = entidad.EstructuraId;
                    cmd.Parameters["p1"].Value = entidad.EmpleadoId;
                    cmd.Parameters["p2"].Value = entidad.AreaId;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar los empleados en una estructura.", ex);
        }
    }

    public bool Limpiar(int estructuraId)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_EstructuraEmpleado_Limpiar(@p0)";

                cmd.Parameters.AddWithValue("p0", estructuraId);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al limpiar las empleados por estructura.", ex);
        }
    }

    #region Constructores

    public EstructuraEmpleadoRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}