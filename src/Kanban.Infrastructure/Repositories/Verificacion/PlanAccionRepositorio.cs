using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificacion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class PlanAccionRepositorio : BaseRepositorio, IPlanAccionRepositorio
{
    public List<PlanAccion> Listar(int verificarId)
    {
        List<PlanAccion> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.verificarid, T1.descripcion, T2.id, T2.codigo ");
            _query.Append("FROM PlanAccion T1 ");
            _query.Append("LEFT JOIN Compromiso T2 ON T1.id = T2.planaccionid ");
            _query.Append("WHERE T1.verificarid = @verificarid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("verificarid", verificarId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new PlanAccion
                        {
                            Id = rd.GetInt32(0),
                            VerificarId = rd.GetInt32(1),
                            Descripcion = rd.GetString(2),
                            Compromiso = rd.IsDBNull(3)
                                ? null
                                : new Domain.Genericos.Compromiso.Compromiso
                                {
                                    Id = rd.GetInt32(3),
                                    Codigo = rd.GetString(4)
                                }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los planes de accion.", ex);
        }
    }

    public PlanAccion? Buscar(int id)
    {
        PlanAccion? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, verificarid, descripcion ");
            _query.Append("FROM PlanAccion ");
            _query.Append("WHERE id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new PlanAccion
                        {
                            Id = rd.GetInt32(0),
                            VerificarId = rd.GetInt32(1),
                            Descripcion = rd.GetString(2)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los planes de accion.", ex);
        }
    }

    public void Guardar(List<PlanAccion> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_PlanAccion_Guardar(@p0, @p1)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });

                foreach (var item in entidades)
                {
                    cmd.Parameters["p0"].Value = item.VerificarId;
                    cmd.Parameters["p1"].Value = item.Descripcion;

                    item.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar los planes de accion.", ex);
        }
    }

    #region Constructores

    public PlanAccionRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}