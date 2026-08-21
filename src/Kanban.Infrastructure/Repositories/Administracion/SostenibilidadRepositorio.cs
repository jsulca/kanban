using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class SostenibilidadRepositorio : BaseRepositorio, ISostenibilidadRepositorio
{
    public List<Sostenibilidad> Listar(int estructuraId)
    {
        List<Sostenibilidad> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.estructuraid, T1.empleadoid, T2.codigo, T2.descripcion, T3.nombre, T3.apellidopaterno, T3.apellidomaterno ");
            _query.Append("FROM Sostenibilidad T1 ");
            _query.Append("INNER JOIN Estructura T2 ON T1.estructuraid = T2.id ");
            _query.Append("INNER JOIN Empleado T3 ON T1.empleadoid = T3.id ");
            _query.Append("WHERE T1.estructuraid = @estructuraid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("estructuraid", estructuraId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Sostenibilidad
                        {
                            EstructuraId = rd.GetInt32(0),
                            EmpleadoId = rd.GetInt32(1),
                            Estructura = new Estructura
                            {
                                Id = rd.GetInt32(0),
                                Codigo = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                                Descripcion = rd.GetString(3)
                            },
                            Empleado = new Empleado
                            {
                                Id = rd.GetInt32(1),
                                Nombre = rd.GetString(4),
                                ApellidoPaterno = rd.GetString(5),
                                ApellidoMaterno = rd.GetString(6)
                            }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los empleados de sostenibilidad de una estructura.", ex);
        }
    }

    public void Guardar(Sostenibilidad entidad)
    {
        try
        {
            Guardar(new List<Sostenibilidad> { entidad });
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar un empleado de sostenibilidad con una área.", ex);
        }
    }

    public void Guardar(List<Sostenibilidad> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Sosteniblidad_Guardar(@p0, @p1)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });

                foreach (var entidad in entidades)
                {
                    cmd.Parameters["p0"].Value = entidad.EstructuraId;
                    cmd.Parameters["p1"].Value = entidad.EmpleadoId;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar los empleados de sostenibilidad en una estructura.", ex);
        }
    }

    public bool Limpiar(int estructuraId)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Sostenibilidad_Limpiar(@p0)";
                cmd.Parameters.AddWithValue("p0", estructuraId);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al limpiar los empleados de sostenibilidad por estructura.", ex);
        }
    }

    #region Constructores

    public SostenibilidadRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}