namespace Kanban.Application.Abstractions;

/// <summary>
///     Confirma en base de datos los cambios acumulados por los repositorios EF.
///     IRepositorioGenerico&lt;T&gt; sólo marca el estado de las entidades;
///     sin una llamada a <see cref="SaveChanges" /> nada se persiste.
/// </summary>
public interface IUnitOfWork
{
    int SaveChanges();
}