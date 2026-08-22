using System.Text;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificaciones;

public class SostenibilidadMesRepositorio : BaseRepositorio, ISostenibilidadMesRepositorio
{
    public List<SostenibilidadMes> Listar(SostenibilidadMesFiltro? filtro)
    {
        var _parametros = new List<NpgsqlParameter>();
        var _queryConditions = new StringBuilder();
        List<SostenibilidadMes> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT estructuraid, empleadoid, tipoverificacionid, anio, nroanio ");
            _query.Append("FROM SostenibilidadMes ");

            if (filtro != null)
            {
                _queryConditions.Append("WHERE 1 = 1 ");

                _queryConditions.Append("AND estructuraid = @estructuraid ");
                _parametros.Add(new NpgsqlParameter("estructuraid", filtro.EstructuraId));

                _queryConditions.Append("AND anio = @anio ");
                _parametros.Add(new NpgsqlParameter("anio", filtro.Anio));

                _query.Append(_queryConditions);
            }

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new SostenibilidadMes
                        {
                            EstructuraId = rd.GetInt32(0),
                            EmpleadoId = rd.GetInt32(1),
                            TipoVerificacionId = rd.GetInt32(2),
                            Anio = rd.GetInt32(3),
                            NroAnio = rd.GetInt32(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException(
                "Ocurrió un problema al listar la informacion por mes de cada personal de sostenibilidad.", ex);
        }
    }

    public void Guardar(List<SostenibilidadMes> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_SostenibilidadMes_Guardar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p3" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4" });

                foreach (var item in entidades)
                {
                    cmd.Parameters["p0"].Value = item.EstructuraId;
                    cmd.Parameters["p1"].Value = item.EmpleadoId;
                    cmd.Parameters["p2"].Value = item.TipoVerificacionId;
                    cmd.Parameters["p3"].Value = item.Anio;
                    cmd.Parameters["p4"].Value = item.NroAnio;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException(
                "Ocurrió un problema al guardar la informacion por año de cada personal de sostenibilidad.", ex);
        }
    }

    #region Constructores

    public SostenibilidadMesRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}