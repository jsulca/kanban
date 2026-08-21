using System.Text;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.Infrastructure.Common;
using Npgsql;

namespace Kanban.Infrastructure.Repositories.Seguridad;

public class IntentoRepositorio : BaseRepositorio, IIntentoRepositorio
{
    public List<Intento> Listar(string usuario, int pageSize)
    {
        var _parametros = new List<NpgsqlParameter>();
        List<Intento> lista = new();
        try
        {
            var _query = new StringBuilder();
            _query.Append("SELECT T1.id, T1.usuario, T1.clave, T1.descripcion, T1.fecharegistro ");
            _query.Append("FROM Intento T1 ");
            _query.Append("WHERE T1.usuario = @usuario ");
            _query.Append("ORDER BY T1.fecharegistro DESC ");
            _query.Append("LIMIT @pagesize ");

            _parametros.Add(new NpgsqlParameter("usuario", usuario));
            _parametros.Add(new NpgsqlParameter("pagesize", pageSize));

            using (var cmd = CreateCommand())
            {
                cmd.CommandText = _query.ToString();
                _parametros.ForEach(x => cmd.Parameters.Add(x));

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Intento
                        {
                            Id = rd.GetFieldValue<int>(0),
                            Usuario = rd.GetFieldValue<string>(1),
                            Clave = !rd.IsDBNull(2) ? rd.GetFieldValue<string>(2) : null,
                            Descripcion = !rd.IsDBNull(3) ? rd.GetFieldValue<string>(3) : null,
                            FechaRegistro = rd.GetFieldValue<DateTime>(4)
                        });
                    rd.Close();
                }
            }

            return lista;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Error al momento de listar los intentos.", ex);
        }
    }

    #region Transacciones

    public bool Guardar(Intento entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad.Usuario);

        var respuesta = false;
        try
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM usp_Intento_Guardar(@p0, @p1, @p2)";

                cmd.Parameters.AddWithValue("p0", entidad.Usuario);
                cmd.Parameters.AddWithValue("p1", entidad.Clave ?? _NullValue);
                cmd.Parameters.AddWithValue("p2", entidad.Descripcion ?? _NullValue);

                entidad.Id = Convert.ToInt32(cmd.ExecuteScalar());

                respuesta = entidad.Id > 0;
            }

            return respuesta;
        }
        catch (Exception ex)
        {
            throw new RepositorioException("Ocurrio un error al momento de guardar el intento.", ex);
        }
    }

    #endregion

    #region Constructores

    public IntentoRepositorio(NpgsqlConnection connection) : base(connection)
    {
    }

    #endregion
}