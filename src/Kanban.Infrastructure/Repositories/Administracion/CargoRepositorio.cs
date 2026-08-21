using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Administracion;

public class CargoRepositorio : BaseRepositorio, ICargoRepositorio
{
    public List<Cargo> Listar()
    {
        List<Cargo> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.codigo, T1.descripcion, T1.activo ");
            _query.Append("FROM Cargo T1 ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Cargo
                        {
                            Id = rd.GetInt32(0),
                            Codigo = !rd.IsDBNull(1) ? rd.GetString(1) : null,
                            Descripcion = rd.GetString(2),
                            Activo = rd.GetBoolean(3)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar los cargos.", ex);
        }
    }

    public Cargo? Buscar(int id)
    {
        Cargo? entidad = null;
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.codigo, T1.descripcion, T1.activo ");
            _query.Append("FROM Cargo T1 ");
            _query.Append("WHERE T1.id = @id");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("id", id);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        entidad = new Cargo
                        {
                            Id = id,
                            Codigo = !rd.IsDBNull(1) ? rd.GetString(1) : null,
                            Descripcion = rd.GetString(2),
                            Activo = rd.GetBoolean(3)
                        };
                    rd.Close();
                }
            }

            return entidad;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al buscar un cargo.", ex);
        }
    }

    public bool Guardar(Cargo entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Cargo_Guardar(@p0, @p1, @p2)";

                cmd.Parameters.AddWithValue("p0", entidad.Codigo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p2", entidad.Activo);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al agregar un nuevo cargo.", ex);
        }
    }

    public bool Actualizar(Cargo entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Descripcion);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Cargo_Actualizar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Codigo?.ToUpper() ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Descripcion.ToUpper());
                cmd.Parameters.AddWithValue("p3", entidad.Activo);

                respuesta = cmd.ExecuteNonQuery() > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar el cargo.", ex);
        }
    }

    #region Constructores

    public CargoRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}