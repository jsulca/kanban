using System.Text;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Verificaciones;

public class CategoriaRepositorio : BaseRepositorio, ICategoriaRepositorio
{
    public List<Categoria> Listar(int verificacionId)
    {
        var lista = new List<Categoria>();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.verificacionid, T1.descripcion, T1.orden, T1.eliminado ");
            _query.Append("FROM Categoria T1 ");
            _query.Append("WHERE T1.verificacionid = @verificacionid ");
            _query.Append("AND  T1.eliminado = FALSE ");

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                cmd.Parameters.AddWithValue("verificacionid", verificacionId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Categoria
                        {
                            Id = rd.GetInt32(0),
                            VerificacionId = rd.GetInt32(1),
                            Descripcion = rd.GetString(2),
                            Orden = rd.GetInt32(3),
                            Eliminado = rd.GetBoolean(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al listar las categorias.", ex);
        }
    }

    public void Guardar(Categoria entidad)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Categoria_Guardar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.VerificacionId);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Orden);
                cmd.Parameters.AddWithValue("p3", entidad.Eliminado);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al guardar una categoria.", ex);
        }
    }

    public void Actualizar(Categoria entidad)
    {
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "CALL usp_Categoria_Actualizar(@p0, @p1, @p2, @p3)";

                cmd.Parameters.AddWithValue("p0", entidad.Id);
                cmd.Parameters.AddWithValue("p1", entidad.Descripcion ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Orden);
                cmd.Parameters.AddWithValue("p3", entidad.Eliminado);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrió un problema al actualizar una categoria.", ex);
        }
    }

    #region Constructores

    public CategoriaRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}