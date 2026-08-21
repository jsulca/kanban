using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class MenuRepositorio : BaseRepositorio, IMenuRepositorio
{
    public List<Menu>? Listar()
    {
        List<Menu>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, padreid, nombre, url, icono, tipo, orden ");
            _query.Append("FROM Menu ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.HasRows)
                    {
                        lista = new List<Menu>();
                        while (rd.Read())
                            lista.Add(new Menu
                            {
                                Id = rd.GetInt32(0),
                                PadreId = !rd.IsDBNull(1) ? rd.GetInt32(1) : (int?)null,
                                Nombre = rd.GetString(2),
                                Url = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                                Icono = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                                Tipo = (TipoMenu)rd.GetInt32(5),
                                Orden = !rd.IsDBNull(6) ? rd.GetInt32(6) : (int?)null
                            });
                    }

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los menus.", ex);
        }
    }

    public Menu? Buscar(int id)
    {
        Menu? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, padreid, nombre, url, icono, tipo, orden ");
            _query.Append("FROM Menu ");
            _query.Append("WHERE id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        entidad = new Menu
                        {
                            Id = rd.GetInt32(0),
                            PadreId = !rd.IsDBNull(1) ? rd.GetInt32(1) : (int?)null,
                            Nombre = rd.GetString(2),
                            Url = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                            Icono = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                            Tipo = (TipoMenu)rd.GetInt32(5),
                            Orden = !rd.IsDBNull(6) ? rd.GetInt32(6) : (int?)null
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

    public void Guardar(Menu entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Menu_Guardar(@p0, @p1, @p2, @p3, @p4, @p5)";

                cmd.Parameters.AddWithValue("p0", entidad.PadreId ?? _NullValue);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre);
                cmd.Parameters.AddWithValue("p2", entidad.Url ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Icono ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.Orden ?? _NullValue);
                cmd.Parameters.AddWithValue("p5", (int)entidad.Tipo);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el menu", ex);
        }
    }

    public void Actualizar(Menu entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Menu_Actualizar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre);
                cmd.Parameters.AddWithValue("p2", entidad.Url ?? _NullValue);
                cmd.Parameters.AddWithValue("p3", entidad.Icono ?? _NullValue);
                cmd.Parameters.AddWithValue("p4", entidad.Orden ?? _NullValue);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el menu", ex);
        }
    }

    #region Constructores

    public MenuRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}