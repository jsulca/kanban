using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class ColorRepositorio : BaseRepositorio, IColorRepositorio
{
    public List<Color> Listar()
    {
        List<Color> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, descripcion, rgba, hex, clase FROM Color");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Color
                        {
                            Id = rd.GetInt32(0),
                            Descripcion = rd.GetString(1),
                            Rgba = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            Hex = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            Clase = !rd.IsDBNull(4) ? rd.GetString(4) : null
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los colores.", ex);
        }
    }

    public Color? Buscar(int id)
    {
        Color? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, descripcion, rgba, hex, clase FROM Color ");
            _query.Append("WHERE id = @id");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Color
                        {
                            Id = id,
                            Descripcion = rd.GetString(1),
                            Rgba = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                            Hex = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            Clase = !rd.IsDBNull(4) ? rd.GetString(4) : null
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar un color.", ex);
        }
    }

    public bool Guardar(Color entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Color_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p1", entidad.Rgba ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Hex ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Clase ?? _NullValue);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar un color.", ex);
        }
    }

    public bool Actualizar(Color entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Color_Actualizar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p2", entidad.Rgba ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Hex ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.Clase ?? _NullValue);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar un color.", ex);
        }
    }

    #region Constructores

    public ColorRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}