namespace Kanban.Infrastructure.Common;

/// <summary>
///     Error al acceder a la base de datos. Envuelve la excepción original en
///     <see cref="Exception.InnerException" />, que el código heredado de .NET
///     Framework descartaba junto con su stack trace.
/// </summary>
public class RepositorioException : Exception
{
    public RepositorioException(string message) : base(message)
    {
    }

    public RepositorioException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
