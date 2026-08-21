using System.Linq.Expressions;

namespace Kanban.Application.Abstractions.Repositories;

/// <summary>
///     Operaciones CRUD genéricas sobre el contexto EF. Los métodos de escritura sólo
///     marcan el estado de la entidad: el commit lo hace IUnitOfWork.SaveChanges().
/// </summary>
public interface IRepositorioGenerico<T> where T : class
{
    T? GetOne(Expression<Func<T, bool>> condicion);

    List<T> GetAllBy(Expression<Func<T, bool>> condicion);

    List<T> GetAll();

    int CountBy(Expression<Func<T, bool>> condicion);

    int Count();

    void Save(T entidad);

    void Save(List<T> entidades);

    void Update(T entidad);

    void Update(List<T> entidades);

    void Delete(T entidad);

    void Delete(List<T> entidades);
}