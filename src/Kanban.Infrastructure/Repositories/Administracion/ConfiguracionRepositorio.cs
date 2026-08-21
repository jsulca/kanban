using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class ConfiguracionRepositorio : BaseRepositorio, IConfiguracionRepositorio
{
    public List<Configuracion> Listar()
    {
        List<Configuracion> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT llave, descripcion, dias FROM Configuracion");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Configuracion
                        {
                            Llave = rd.GetString(0),
                            Descripcion = rd.GetString(1),
                            Dias = rd.GetInt32(2)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar configuraciones.", ex);
        }
    }

    public Configuracion? Buscar(string llave)
    {
        Configuracion? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT llave, descripcion, dias FROM Configuracion ");
            _query.Append("WHERE llave = @llave");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("llave", llave);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Configuracion
                        {
                            Llave = rd.GetString(0),
                            Descripcion = rd.GetString(1),
                            Dias = rd.GetInt32(2)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar la configuracion.", ex);
        }
    }

    public void Actualizar(Configuracion entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Llave);

        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Configuracion_Actualizar(@p0, @p1)";

                cmd.Parameters.AddWithValue("p0", entidad.Llave);
                cmd.Parameters.AddWithValue("p1", entidad.Dias);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar la configuración.", ex);
        }
    }

    public void ActualizarVencimiento()
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Configuracion_ActualizarUsuarioConVencimiento(@p0)";

                cmd.Parameters.AddWithValue("p0", ConfiguracionMaestro.RENOVACION_CLAVE);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar los dias de vencimiento en los usuarios.", ex);
        }
    }

    #region Constructores

    public ConfiguracionRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}