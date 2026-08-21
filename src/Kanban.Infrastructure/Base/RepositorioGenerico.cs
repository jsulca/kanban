using System.Linq.Expressions;
using Kanban.Application.Abstractions.Repositories;
using Kanban.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Base;

public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : class
{
    protected readonly EFContexto _contexto;

    public RepositorioGenerico(EFContexto contexto)
    {
        _contexto = contexto;
    }

    public T? GetOne(Expression<Func<T, bool>> condicion)
    {
        return _contexto.Set<T>().Where(condicion).SingleOrDefault();
    }

    public List<T> GetAllBy(Expression<Func<T, bool>> condicion)
    {
        return _contexto.Set<T>().Where(condicion).ToList();
    }

    public List<T> GetAll()
    {
        return _contexto.Set<T>().ToList();
    }

    public int CountBy(Expression<Func<T, bool>> condicion)
    {
        return _contexto.Set<T>().Where(condicion).Count();
    }

    public int Count()
    {
        return _contexto.Set<T>().Count();
    }


    public void Save(T entidad)
    {
        _contexto.Entry(entidad).State = EntityState.Added;
    }

    public void Save(List<T> entidades)
    {
        entidades.ForEach(entidad => _contexto.Entry(entidad).State = EntityState.Added);
    }

    public void Update(T entidad)
    {
        _contexto.Entry(entidad).State = EntityState.Modified;
    }

    public void Update(List<T> entidades)
    {
        entidades.ForEach(entidad => _contexto.Entry(entidad).State = EntityState.Modified);
    }

    public void Delete(T entidad)
    {
        _contexto.Entry(entidad).State = EntityState.Deleted;
    }

    public void Delete(List<T> entidades)
    {
        entidades.ForEach(entidad => _contexto.Entry(entidad).State = EntityState.Deleted);
    }
}