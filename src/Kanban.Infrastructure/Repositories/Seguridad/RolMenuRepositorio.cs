using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class RolMenuRepositorio : BaseRepositorio, IRolMenuRepositorio
{
    public List<RolMenu>? Listar(int rolId)
    {
        List<RolMenu>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.rolid, T1.menuid, T2.padreid, T2.nombre, T2.url, T2.icono, T2.tipo, T2.orden ");
            _query.Append("FROM RolMenu T1 ");
            _query.Append("INNER JOIN Menu T2 ON T1.menuid = T2.id ");
            _query.Append("WHERE T1.rolid = @rolid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("rolid", rolId);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.HasRows)
                    {
                        lista = new List<RolMenu>();
                        while (rd.Read())
                            lista.Add(new RolMenu
                            {
                                RolId = rd.GetInt32(0),
                                MenuId = rd.GetInt32(1),
                                Menu = new Menu
                                {
                                    Id = rd.GetInt32(1),
                                    PadreId = !rd.IsDBNull(2) ? rd.GetInt32(2) : (int?)null,
                                    Nombre = rd.GetString(3),
                                    Url = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                                    Icono = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                                    Tipo = (TipoMenu)rd.GetInt32(6),
                                    Orden = !rd.IsDBNull(7) ? rd.GetInt32(7) : (int?)null
                                }
                            });
                    }

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los menus del rol.", ex);
        }
    }

    public void Guardar(RolMenu entidad)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_RolMenu_Guardar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.RolId);
                cmd.Parameters.AddWithValue("p1", entidad.MenuId);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el menu del rol.", ex);
        }
    }

    public void Limpiar(int rolId)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_RolMenu_Limpiar(@p0)";
                cmd.Parameters.AddWithValue("p0", rolId);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error limpiar los menus del rol.", ex);
        }
    }

    #region Constructores

    public RolMenuRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}