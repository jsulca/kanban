using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class EstructuraAreaRepositorio : BaseRepositorio, IEstructuraAreaRepositorio
{
    public List<EstructuraArea> Listar(int estructuraId)
    {
        List<EstructuraArea> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.estructuraid, T1.areaid, T2.descripcion, T2.colorfondoid, T3.clase, T3.rgba ");
            _query.Append("FROM EstructuraArea T1 ");
            _query.Append("INNER JOIN Area T2 ON T1.areaid = T2.id ");
            _query.Append("INNER JOIN Color T3 ON T2.colorfondoid = T3.id ");
            _query.Append("WHERE T1.estructuraid = @estructuraid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("estructuraid", estructuraId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new EstructuraArea
                        {
                            EstructuraId = rd.GetInt32(0),
                            AreaId = rd.GetInt32(1),
                            Area = new Area
                            {
                                Id = rd.GetInt32(1),
                                Descripcion = rd.GetString(2),
                                ColorFondoId = rd.GetInt32(3),
                                ColorFondo = new Color
                                {
                                    Clase = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                                    Rgba = !rd.IsDBNull(5) ? rd.GetString(5) : null
                                }
                            }
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las áreas de una estructura.", ex);
        }
    }

    public void Guardar(EstructuraArea entidad)
    {
        try
        {
            Guardar(new List<EstructuraArea> { entidad });
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una estructura con una área.", ex);
        }
    }

    public void Guardar(List<EstructuraArea> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_EstructuraArea_Guardar(@p0, @p1)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });

                foreach (var entidad in entidades)
                {
                    cmd.Parameters["p0"].Value = entidad.EstructuraId;
                    cmd.Parameters["p1"].Value = entidad.AreaId;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar las áreas en una estructura.", ex);
        }
    }

    public bool Limpiar(int estructuraId)
    {
        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_EstructuraArea_Limpiar(@p0)";

                cmd.Parameters.AddWithValue("p0", estructuraId);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al limpiar las áreas por estructura.", ex);
        }
    }

    #region Constructores

    public EstructuraAreaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}