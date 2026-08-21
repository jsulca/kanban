using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class InstanciaRepositorio : BaseRepositorio, IInstanciaRepositorio
{
    public List<Instancia> Listar()
    {
        List<Instancia> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.abreviatura, T1.descripcion, T1.colorfondoid, T1.colortextoid ");
            _query.Append("FROM Instancia T1 ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Instancia
                        {
                            Id = rd.GetInt32(0),
                            Abreviatura = !rd.IsDBNull(1) ? rd.GetString(1) : null,
                            Descripcion = rd.GetString(2),
                            ColorFondoId = rd.GetInt32(3),
                            ColorTextoId = rd.GetInt32(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las instancias.", ex);
        }
    }

    public Instancia? Buscar(int id)
    {
        Instancia? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.abreviatura, T1.descripcion, T1.colorfondoid, T1.colortextoid ");
            _query.Append("FROM Instancia T1 ");
            _query.Append("WHERE T1.id = @id ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Instancia
                        {
                            Id = rd.GetInt32(0),
                            Abreviatura = !rd.IsDBNull(1) ? rd.GetString(1) : null,
                            Descripcion = rd.GetString(2),
                            ColorFondoId = rd.GetInt32(3),
                            ColorTextoId = rd.GetInt32(4)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar una instancia.", ex);
        }
    }

    public bool Guardar(Instancia entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Instancia_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Abreviatura?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p2", entidad.ColorFondoId);
                cmd.Parameters.AddWithValue("p3", entidad.ColorTextoId);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una instancia.", ex);
        }
    }

    public bool Actualizar(Instancia entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Instancia_Actualizar(@p0, @p1, @p2, @p3, @p4)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Abreviatura?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p3", entidad.ColorFondoId);
                cmd.Parameters.AddWithValue("p4", entidad.ColorTextoId);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar una instancia.", ex);
        }
    }

    #region Constructores

    public InstanciaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}