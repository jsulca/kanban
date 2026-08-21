using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificacion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificacion;

public class ConfirmadorComentarioRepositorio : BaseRepositorio, IConfirmadorComentarioRepositorio
{
    public List<ConfirmadorComentario> Listar(ConfirmadorComentarioFiltro? filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<ConfirmadorComentario> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT estructuraid, empleadoid, anio, mes, comentario ");
            _query.Append("FROM ConfirmadorComentario ");

            if (filtro != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                _queryConditions.Append("AND estructuraid = @estructuraid ");
                _parametros.Add(new NpgsqlParameter("estructuraid", filtro.EstructuraId));

                _queryConditions.Append("AND anio = @anio ");
                _parametros.Add(new NpgsqlParameter("anio", filtro.Anio));

                _queryConditions.Append("AND mes = @mes ");
                _parametros.Add(new NpgsqlParameter("mes", filtro.Mes));

                _query.Append(_queryConditions);
            }

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new ConfirmadorComentario
                        {
                            EstructuraId = rd.GetInt32(0),
                            EmpleadoId = rd.GetInt32(1),
                            Anio = rd.GetInt32(2),
                            Mes = rd.GetInt32(3),
                            Comentario = !rd.IsDBNull(4) ? rd.GetString(4) : null
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los comentarios por mes de cada confirmador.", ex);
        }
    }

    public void Guardar(List<ConfirmadorComentario> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_ConfirmadorComentario_Guardar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p3" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4" });

                foreach (var item in entidades)
                {
                    cmd.Parameters["p0"].Value = item.EstructuraId;
                    cmd.Parameters["p1"].Value = item.EmpleadoId;
                    cmd.Parameters["p2"].Value = item.Anio;
                    cmd.Parameters["p3"].Value = item.Mes;
                    cmd.Parameters["p4"].Value = item.Comentario ?? _NullValue;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar los comentarios por mes de cada confirmador.", ex);
        }
    }

    #region Constructores

    public ConfirmadorComentarioRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}