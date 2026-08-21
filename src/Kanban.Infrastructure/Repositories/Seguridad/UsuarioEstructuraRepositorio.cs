using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class UsuarioEstructuraRepositorio : BaseRepositorio, IUsuarioEstructuraRepositorio
{
    public List<UsuarioEstructura> Listar(int usuarioId)
    {
        List<UsuarioEstructura>? lista = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append(
                "SELECT T1.usuarioid, T1.estructuraid, T1.acceso, T2.padreid, T2.codigo, T2.descripcion, T2.tablero ");
            _query.Append("FROM UsuarioEstructura T1 ");
            _query.Append("INNER JOIN Estructura T2 ON T1.estructuraid = T2.id ");
            _query.Append("WHERE T1.usuarioid = @usuarioid ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("usuarioid", usuarioId);

                using (var rd = cmd.ExecuteReader())
                {
                    lista = new List<UsuarioEstructura>();

                    while (rd.Read())
                        lista.Add(new UsuarioEstructura
                        {
                            UsuarioId = rd.GetInt32(0),
                            EstructuraId = rd.GetInt32(1),
                            Acceso = rd.GetBoolean(2),
                            Estructura = new Estructura
                            {
                                Id = rd.GetInt32(1),
                                PadreId = !rd.IsDBNull(3) ? rd.GetInt32(3) : (int?)null,
                                Codigo = !rd.IsDBNull(4) ? rd.GetString(4) : null,
                                Descripcion = rd.GetString(5),
                                Tablero = rd.GetBoolean(6)
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

    public void Guardar(List<UsuarioEstructura> estructuras)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_UsuarioEstructura_Guardar(@p0, @p1, @p2)";

                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p0" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2" });

                foreach (var item in estructuras)
                {
                    cmd.Parameters["p0"].Value = item.UsuarioId;
                    cmd.Parameters["p1"].Value = item.EstructuraId;
                    cmd.Parameters["p2"].Value = item.Acceso;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al guardar las estructuras en un usuario.", ex);
        }
    }

    public void Limpiar(int usuarioId)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_UsuarioEstructura_Limpiar(@p0)";
                cmd.Parameters.AddWithValue("p0", usuarioId);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error limpiar las estructuras de un usuario.", ex);
        }
    }

    #region Constructores

    public UsuarioEstructuraRepositorio(NpgsqlConnection cn) : base(cn)
    {
    }

    #endregion
}