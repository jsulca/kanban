using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class AdjuntoRepositorio : BaseRepositorio, IAdjuntoRepositorio
{
    public Adjunto? Buscar(int id)
    {
        Adjunto? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT id, nombre, ruta, tipoarchivo, tamano FROM Adjunto ");
            _query.Append("WHERE id = @id");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Adjunto
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            Ruta = rd.GetString(2),
                            TipoArchivo = rd.GetString(3),
                            Tamano = rd.GetInt32(4)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar un adjunto.", ex);
        }
    }

    public bool Guardar(Adjunto entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Nombre);
        ArgumentNullException.ThrowIfNull(entidad.Ruta);
        ArgumentNullException.ThrowIfNull(entidad.TipoArchivo);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Adjunto_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Nombre);
                cmd.Parameters.AddWithValue("p1", entidad.Ruta);
                cmd.Parameters.AddWithValue("p2", entidad.TipoArchivo);
                cmd.Parameters.AddWithValue("p3", entidad.Tamano);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar un archivo adjunto.", ex);
        }
    }

    #region Constructores

    public AdjuntoRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}