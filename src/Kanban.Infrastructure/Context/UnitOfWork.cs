using Kanban.Application.Abstractions;

namespace Kanban.Infrastructure.Context;

/// <summary>
///     Confirma los cambios que los repositorios EF han marcado sobre el contexto.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly EFContexto _contexto;

    public UnitOfWork(EFContexto contexto)
    {
        _contexto = contexto;
    }

    public int SaveChanges()
    {
        return _contexto.SaveChanges();
    }
}
