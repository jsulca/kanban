using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.UseCases.Verificacion;

public class VerificacionLogica(
    IVerificacionRepositorio verificaciones,
    ICategoriaRepositorio categorias,
    IPreguntaRepositorio preguntas,
    IRespuestaRepositorio respuestas,
    ITransacciones transacciones)
    : IVerificacionLogica
{
    public PagedResult<Domain.Genericos.Verificacion.Verificacion> ListarPorPagina(VerificacionFiltro filtro, int page,
        int pageSize)
    {
        return verificaciones.ListarPorPagina(filtro, page, pageSize);
    }

    public List<Domain.Genericos.Verificacion.Verificacion> Listar()
    {
        return verificaciones.Listar();
    }

    public Domain.Genericos.Verificacion.Verificacion? Buscar(int id, bool conDetalles = false)
    {
        var entidad = verificaciones.Buscar(id);

        if (conDetalles && entidad is not null)
        {
            var categorias1 = categorias.Listar(id);
            var preguntas1 = preguntas.Listar(id);
            var respuestas1 = respuestas.Listar(id);

            foreach (var pregunta in preguntas1)
                pregunta.Respuestas = respuestas1.Where(x => x.PreguntaId == pregunta.Id).ToList();

            foreach (var categoria in categorias1)
                categoria.Preguntas = preguntas1.Where(x => x.CategoriaId == categoria.Id).ToList();

            entidad.Categorias = categorias1;
        }

        return entidad;
    }

    public void Guardar(Domain.Genericos.Verificacion.Verificacion entidad)
    {
        transacciones.Ejecutar(() =>
        {
            if (!verificaciones.Guardar(entidad) || entidad.Categorias is null) return;

            foreach (var categoria in entidad.Categorias)
            {
                categoria.VerificacionId = entidad.Id;
                categorias.Guardar(categoria);

                if (categoria.Preguntas is null) continue;

                foreach (var pregunta in categoria.Preguntas)
                {
                    pregunta.CategoriaId = categoria.Id;

                    if (!preguntas.Guardar(pregunta) || pregunta.Respuestas is null) continue;

                    foreach (var respuesta in pregunta.Respuestas)
                    {
                        respuesta.PreguntaId = pregunta.Id;
                        respuestas.Guardar(respuesta);
                    }
                }
            }
        });
    }

    public void Actualizar(Domain.Genericos.Verificacion.Verificacion entidad)
    {
        transacciones.Ejecutar(() =>
        {
            verificaciones.Actualizar(entidad);

            if (entidad.Categorias is null) return;

            foreach (var categoria in entidad.Categorias)
            {
                categoria.VerificacionId = entidad.Id;

                if (categoria.Id > 0) categorias.Actualizar(categoria);
                else categorias.Guardar(categoria);

                if (categoria.Preguntas is null) continue;

                foreach (var pregunta in categoria.Preguntas)
                {
                    if (pregunta.Id > 0)
                    {
                        preguntas.Actualizar(pregunta);
                    }
                    else
                    {
                        pregunta.CategoriaId = categoria.Id;
                        preguntas.Guardar(pregunta);
                    }

                    if (pregunta.Respuestas is null) continue;

                    respuestas.Limpiar(pregunta.Id);

                    foreach (var respuesta in pregunta.Respuestas)
                    {
                        respuesta.PreguntaId = pregunta.Id;
                        respuestas.Guardar(respuesta);
                    }
                }
            }
        });
    }
}