using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class RolControlRepositorio : BaseRepositorio, IRolControlRepositorio
{
    public List<RolControl> Listar(int rolId)
    {
        List<RolControl>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.rolid, T1.controlid, T2.paginaid, T2.nombre ");
            _query.Append("FROM RolControl T1 ");
            _query.Append("INNER JOIN Control T2 ON T1.controlid = T2.id ");
            _query.Append("WHERE T1.rolid = @rolid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("rolid", rolId);

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<RolControl>();
                    while (rd.Read())
                        lista.Add(new RolControl
                        {
                            RolId = rd.GetInt32(0),
                            ControlId = rd.GetInt32(1),
                            Control = new()
                            {
                                Id = rd.GetInt32(1),
                                PaginaId = rd.GetInt32(2),
                                Nombre = rd.GetString(3)
                            }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los controles del rol.", ex);
        }
    }

    public void Guardar(RolControl entidad)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_RolControl_Guardar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.RolId);
                cmd.Parameters.AddWithValue("p1", entidad.ControlId);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el control del rol.", ex);
        }
    }

    public void Limpiar(int rolId)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_RolControl_Limpiar(@p0)";
                cmd.Parameters.AddWithValue("p0", rolId);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error limpiar los controles del rol.", ex);
        }
    }

    #region Constructores

    public RolControlRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}