using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class RolPaginaRepositorio : BaseRepositorio, IRolPaginaRepositorio
{
    public List<RolPagina> Listar(int rolId)
    {
        List<RolPagina>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.rolid, T1.paginaid, T2.nombre, T2.area, T2.controlador, T2.accion ");
            _query.Append("FROM RolPagina T1 ");
            _query.Append("INNER JOIN Pagina T2 ON T1.paginaid = T2.id ");
            _query.Append("WHERE T1.rolid = @rolid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("rolid", rolId);

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<RolPagina>();
                    while (rd.Read())
                        lista.Add(new RolPagina
                        {
                            RolId = rd.GetInt32(0),
                            PaginaId = rd.GetInt32(1),
                            Pagina = new Pagina
                            {
                                Id = rd.GetInt32(1),
                                Nombre = rd.GetString(2),
                                Area = !rd.IsDBNull(3) ? rd.GetString(3) : null,
                                Controlador = rd.GetString(4),
                                Accion = rd.GetString(5)
                            }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar las paginas del rol.", ex);
        }
    }

    public void Guardar(RolPagina entidad)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_RolPagina_Guardar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.RolId);
                cmd.Parameters.AddWithValue("p1", entidad.PaginaId);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar la pagina del rol.", ex);
        }
    }

    public void Limpiar(int rolId)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_RolPagina_Limpiar(@p0)";
                cmd.Parameters.AddWithValue("p0", rolId);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error limpiar las paginas del rol.", ex);
        }
    }

    #region Constructores

    public RolPaginaRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}