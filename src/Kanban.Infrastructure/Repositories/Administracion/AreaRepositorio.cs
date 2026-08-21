using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class AreaRepositorio : BaseRepositorio, IAreaRepositorio
{
    public List<Area> Listar()
    {
        List<Area> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.codigo, T1.descripcion, T1.colorfondoid, T1.colortextoid FROM Area T1");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Area
                        {
                            Id = rd.GetInt32(0),
                            Codigo = !rd.IsDBNull(1) ? rd.GetString(1) : null,
                            Descripcion = rd.GetString(2),
                            ColorFondoId = rd.GetInt32(3),
                            ColorTextoId = rd.GetInt32(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las áreas.", ex);
        }
    }

    public Area? Buscar(int id)
    {
        Area? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.codigo, T1.descripcion, T1.colorfondoid, T1.colortextoid, ");
            _query.Append("T2.clase, T2.hex, T2.rgba, ");
            _query.Append("T3.clase, T3.hex, T3.rgba ");
            _query.Append("FROM Area T1 ");
            _query.Append("INNER JOIN Color T2 ON T1.colorfondoid = T2.id ");
            _query.Append("INNER JOIN Color T3 ON T1.colortextoid = T3.id ");
            _query.Append("WHERE T1.id = @id");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Area
                        {
                            Id = id,
                            Codigo = !rd.IsDBNull(0) ? rd.GetString(0) : null,
                            Descripcion = rd.GetString(1),
                            ColorFondoId = rd.GetInt32(2),
                            ColorTextoId = rd.GetInt32(3),
                            ColorFondo = new Color
                            {
                                Clase = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                                Hex = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                                Rgba = !rd.IsDBNull(6) ? rd.GetString(6) : null
                            },
                            ColorTexto = new Color
                            {
                                Clase = !rd.IsDBNull(7) ? rd.GetString(7) : null,
                                Hex = !rd.IsDBNull(8) ? rd.GetString(8) : null,
                                Rgba = !rd.IsDBNull(9) ? rd.GetString(9) : null
                            }
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar una área.", ex);
        }
    }

    public bool Guardar(Area entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Area_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Codigo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p2", entidad.ColorFondoId);
                cmd.Parameters.AddWithValue("p3", entidad.ColorTextoId);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una área.", ex);
        }
    }

    public bool Actualizar(Area entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Area_Actualizar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Codigo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p3", entidad.ColorFondoId);
                cmd.Parameters.AddWithValue("p4", entidad.ColorTextoId);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar una área.", ex);
        }
    }

    #region Constructores

    public AreaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}