using System.Data;
using Npgsql;

namespace Kanban.Infrastructure.Common;

public class BaseRepositorio
{
    private readonly NpgsqlConnection _cn;

    protected readonly object _NullValue = DBNull.Value;

    protected BaseRepositorio(NpgsqlConnection connection)
    {
        _cn = connection;
    }

    /// <summary>
    ///     Crea un comando sobre la conexión del repositorio, abriéndola si hace falta.
    ///     Con DI la conexión llega cerrada desde el <c>NpgsqlDataSource</c>; en .NET
    ///     Framework la abría quien construía el repositorio.
    /// </summary>
    protected NpgsqlCommand CreateCommand()
    {
        if (_cn.State == ConnectionState.Closed) _cn.Open();

        return _cn.CreateCommand();
    }
}
