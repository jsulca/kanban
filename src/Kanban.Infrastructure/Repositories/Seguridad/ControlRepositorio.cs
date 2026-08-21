using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class ControlRepositorio : BaseRepositorio, IControlRepositorio
{
    public List<Control>? ListarPorPagina(int paginaid)
    {
        List<Control>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, paginaid, nombre ");
            _query.Append("FROM Control ");
            _query.Append("WHERE paginaid = @paginaid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("paginaid", paginaid);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.HasRows)
                    {
                        lista = new List<Control>();
                        while (rd.Read())
                            lista.Add(new Control
                            {
                                Id = rd.GetInt32(0),
                                PaginaId = rd.GetInt32(1),
                                Nombre = rd.GetString(2)
                            });
                    }

                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al listar los controles.", ex);
        }
    }

    public void Guardar(Control entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Control_Guardar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.PaginaId);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar el control", ex);
        }
    }

    public void Actualizar(Control entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Control_Actualizar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Nombre);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al actualizar el control", ex);
        }
    }

    public void Eliminar(int id)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Control_Eliminar(@p0)";
                cmd.Parameters.AddWithValue("p0", id);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al eliminar el control", ex);
        }
    }

    #region Constructores

    public ControlRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}