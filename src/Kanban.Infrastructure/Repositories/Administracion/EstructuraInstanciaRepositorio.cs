using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class EstructuraInstanciaRepositorio : BaseRepositorio, IEstructuraInstanciaRepositorio
{
    public List<EstructuraInstancia> Listar(int estructuraId)
    {
        List<EstructuraInstancia> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT estructuraid, instanciaid ");
            _query.Append("FROM EstructuraInstancia ");
            _query.Append("WHERE estructuraid = @estructuraid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("estructuraid", estructuraId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new EstructuraInstancia
                        {
                            EstructuraId = rd.GetInt32(0),
                            InstanciaId = rd.GetInt32(1)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las instancias de una estructura.", ex);
        }
    }

    public List<EstructuraInstancia> ListarInstancia(int estructuraId)
    {
        List<EstructuraInstancia> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.estructuraid, T1.instanciaid, T2.abreviatura, T2.descripcion, T2.colorfondoid, T3.rgba, T3.hex, T3.clase, T2.colortextoid, T4.rgba, T4.hex, T4.clase ");
            _query.Append("FROM EstructuraInstancia T1 ");
            _query.Append("INNER JOIN Instancia T2 ON T1.instanciaid = T2.id ");
            _query.Append("INNER JOIN Color T3 ON T2.colorfondoid = T3.id ");
            _query.Append("INNER JOIN Color T4 ON T2.colortextoid = T4.id ");
            _query.Append("WHERE T1.estructuraid = @estructuraid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("estructuraid", estructuraId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new EstructuraInstancia
                        {
                            EstructuraId = rd.GetInt32(0),
                            InstanciaId = rd.GetInt32(1),
                            Instancia = new Instancia
                            {
                                Abreviatura = !rd.IsDBNull(2) ? rd.GetString(2) : null,
                                Descripcion = rd.GetString(3),
                                ColorFondoId = rd.GetInt32(4),
                                ColorFondo = new Color
                                {
                                    Rgba = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                                    Hex = !rd.IsDBNull(6) ? rd.GetString(6) : null,
                                    Clase = !rd.IsDBNull(7) ? rd.GetString(7) : null
                                },
                                ColorTextoId = rd.GetInt32(8),
                                ColorTexto = new Color
                                {
                                    Rgba = !rd.IsDBNull(9) ? rd.GetString(9) : null,
                                    Hex = !rd.IsDBNull(10) ? rd.GetString(10) : null,
                                    Clase = !rd.IsDBNull(11) ? rd.GetString(11) : null
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
            throw new RepositorioException("Ocurrió un problema al listar las instancias de una estructura.", ex);
        }
    }

    public void Guardar(EstructuraInstancia entidad)
    {
        try
        {
            Guardar(new List<EstructuraInstancia> { entidad });
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una instancia en una estructura.", ex);
        }
    }

    public void Guardar(List<EstructuraInstancia> entidades)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_EstructuraInstancia_Guardar(@p0, @p1)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });

                foreach (var entidad in entidades)
                {
                    cmd.Parameters["p0"].Value = entidad.EstructuraId;
                    cmd.Parameters["p1"].Value = entidad.InstanciaId;

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
                cmd.CommandText = "CALL usp_EstructuraInstancia_Limpiar(@p0)";
                cmd.Parameters.AddWithValue("p0", estructuraId);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al limpiar las instancias por estructura.", ex);
        }
    }

    #region Constructores

    public EstructuraInstanciaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}