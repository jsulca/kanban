using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.Repositories.Compromiso;
using Kanban.Application.Abstractions.UseCases.Compromiso;
using Kanban.Application.Common;
using Kanban.Domain;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Compromisos;

namespace Kanban.Application.UseCases.Compromisos;

public class CompromisoLogica(
    ICompromisoRepositorio compromisos,
    ICompromisoEstadoRepositorio estados,
    ICompromisoInstanciaRepositorio instancias,
    IAreaRepositorio areas,
    IEmpleadoRepositorio empleados,
    IAlertaRepositorio alertas,
    IAdjuntoEFRepositorio adjuntosEf,
    ITransacciones transacciones,
    IUnitOfWork unitOfWork)
    : ICompromisoLogica
{
    public PagedResult<Compromiso> ListarPorPagina(CompromisoFiltro filtro, int page,
        int pageSize)
    {
        return compromisos.ListarPorPagina(filtro, page, pageSize);
    }

    public List<Compromiso> Listar(CompromisoFiltro filtro)
    {
        return compromisos.Listar(filtro) ?? [];
    }

    public Compromiso? Buscar(int id, bool conDetalles = false)
    {
        var entidad = compromisos.Buscar(id);

        if (entidad is null) return null;

        if (entidad.AreaId.HasValue)
            entidad.Area = areas.Buscar(entidad.AreaId.Value);

        if (conDetalles)
        {
            entidad.Estados = estados.Listar(id);
            entidad.Instancias = instancias.Listar(id);
        }

        return entidad;
    }

    public IndicadorCompromisos Indicador(int tableroId, DateTime fechaDesde, DateTime fechaHasta)
    {
        return new IndicadorCompromisos(
            compromisos.IndicadorPorEstado_1_1(tableroId, fechaHasta),
            compromisos.IndicadorPorEstado_1_2(tableroId, fechaDesde, fechaHasta),
            compromisos.PorTablero(tableroId));
    }

    public ExportacionCompromisos Exportar(CompromisoFiltro filtro)
    {
        var compromisos1 = compromisos.Exportar(filtro);

        if (compromisos1.Count == 0)
            return new ExportacionCompromisos(compromisos1, [], []);

        var ids = compromisos1.Select(x => x.Id).ToArray();

        return new ExportacionCompromisos(compromisos1, estados.Exportar(ids), instancias.Exportar(ids));
    }

    public void Guardar(Compromiso entidad, int? usuarioId = null, int? empleadoId = null)
    {
        // la foto se persiste por EF y fuera de la transacción ADO, igual que en .NET Framework
        if (entidad.Foto != null)
        {
            adjuntosEf.Save(entidad.Foto);
            unitOfWork.SaveChanges();

            entidad.FotoId = entidad.Foto.Id;
        }

        transacciones.Ejecutar(() =>
        {
            entidad.Codigo = (compromisos.Contar(entidad.TableroId) + 1).ToString("D10");

            if (compromisos.Guardar(entidad))
                estados.Guardar(new CompromisoEstado
                {
                    CompromisoId = entidad.Id,
                    Estado = EstadoCompromiso.NUEVO,
                    UsuarioId = usuarioId,
                    EmpleadoId = empleadoId
                });
        });
    }

    public void Actualizar(Compromiso entidad)
    {
        transacciones.Ejecutar(() => compromisos.Actualizar(entidad));
    }

    public void CambiarEstado(Compromiso entidad, string? motivo = null,
        int? usuarioId = null,
        int? empleadoId = null)
    {
        transacciones.Ejecutar(() =>
        {
            if (compromisos.CambiarEstado(entidad))
            {
                var estado = new CompromisoEstado
                {
                    CompromisoId = entidad.Id,
                    Estado = entidad.Estado,
                    Motivo = motivo,
                    UsuarioId = usuarioId,
                    EmpleadoId = empleadoId
                };

                if (entidad.Estado == EstadoCompromiso.PROGRAMADO)
                {
                    estado.Motivo = DescribirAsignacion(entidad, "programado", entidad.FechaProgramacion);
                    alertas.Guardar(new Alerta
                    {
                        CompromisoId = entidad.Id,
                        EmpleadoId = entidad.ResponsableId!.Value
                    });
                }

                if (entidad.Estado == EstadoCompromiso.REPROGRAMADO)
                {
                    estado.Motivo = DescribirAsignacion(entidad, "reprogramado", entidad.FechaReprogramacion);
                    alertas.Guardar(new Alerta
                    {
                        CompromisoId = entidad.Id,
                        EmpleadoId = entidad.ResponsableId!.Value
                    });
                }

                estados.Guardar(estado);
            }

            if (entidad.Estado == EstadoCompromiso.PENDIENTE)
                compromisos.ReiniciarFecha(entidad.Id);
        });
    }

    public void AsignarAutomatico(Compromiso entidad, int? usuarioId = null,
        int? empleadoId = null)
    {
        transacciones.Ejecutar(() =>
        {
            if (compromisos.CambiarEstado(entidad))
            {
                compromisos.Asignar(entidad);
                estados.Guardar(new CompromisoEstado
                {
                    CompromisoId = entidad.Id,
                    Estado = entidad.Estado,
                    Motivo = entidad.Accion,
                    UsuarioId = usuarioId,
                    EmpleadoId = empleadoId
                });
            }
        });
    }

    public void CambiarInstancia(int id, string motivo, int instanciaId, int? usuarioId = null,
        int? empleadoId = null)
    {
        transacciones.Ejecutar(() =>
        {
            if (compromisos.CambiarInstancia(id, instanciaId))
            {
                compromisos.ReiniciarFecha(id);
                instancias.Guardar(new CompromisoInstancia
                {
                    CompromisoId = id,
                    InstanciaId = instanciaId,
                    Motivo = motivo,
                    UsuarioId = usuarioId,
                    EmpleadoId = empleadoId
                });
            }
        });
    }

    public void Asignar(Compromiso entidad, int? usuarioId = null, int? empleadoId = null)
    {
        transacciones.Ejecutar(() =>
        {
            var compromiso = compromisos.Buscar(entidad.Id)
                             ?? throw new InvalidOperationException($"No existe el compromiso {entidad.Id}.");

            compromisos.Asignar(entidad);

            if (entidad.Estado is EstadoCompromiso.POR_VERIFICAR or EstadoCompromiso.FINALIZADO)
            {
                if (compromiso.Estado != entidad.Estado)
                {
                    compromiso.Estado = entidad.Estado;
                    if (compromisos.CambiarEstado(entidad))
                        estados.Guardar(new CompromisoEstado
                        {
                            CompromisoId = entidad.Id,
                            Estado = entidad.Estado,
                            UsuarioId = usuarioId,
                            EmpleadoId = empleadoId
                        });
                }
            }
            else if (!entidad.InstanciaId.HasValue &&
                     entidad.Estado is EstadoCompromiso.PROGRAMADO or EstadoCompromiso.REPROGRAMADO)
            {
                if (entidad.AreaId.HasValue && entidad.ResponsableId.HasValue &&
                    (compromiso.AreaId != entidad.AreaId || compromiso.ResponsableId != entidad.ResponsableId))
                {
                    var compromisoEstado = new CompromisoEstado
                    {
                        CompromisoId = entidad.Id,
                        UsuarioId = usuarioId,
                        EmpleadoId = empleadoId,
                        Estado = entidad.Estado
                    };

                    // ojo: la primera rama mira el estado anterior y la segunda el nuevo,
                    // tal cual venía de .NET Framework
                    if (compromiso.Estado == EstadoCompromiso.PROGRAMADO)
                    {
                        compromisoEstado.Motivo =
                            DescribirAsignacion(entidad, "programado", entidad.FechaProgramacion);
                        alertas.Guardar(new Alerta
                        {
                            CompromisoId = entidad.Id,
                            EmpleadoId = entidad.ResponsableId.Value
                        });
                    }
                    else if (entidad.Estado == EstadoCompromiso.REPROGRAMADO)
                    {
                        compromisoEstado.Motivo =
                            DescribirAsignacion(entidad, "reprogramado", entidad.FechaReprogramacion);
                        alertas.Guardar(new Alerta
                        {
                            CompromisoId = entidad.Id,
                            EmpleadoId = entidad.ResponsableId.Value
                        });
                    }

                    estados.Guardar(compromisoEstado);
                }
            }
        });
    }

    public void FueraFecha()
    {
        transacciones.Ejecutar(() => compromisos.FueraFecha());
    }

    /// <summary>
    ///     Texto del motivo cuando un compromiso se programa o reprograma.
    /// </summary>
    private string DescribirAsignacion(Compromiso entidad, string verbo, DateTime? fecha)
    {
        var empleado = empleados.Buscar(entidad.ResponsableId!.Value)
                       ?? throw new InvalidOperationException(
                           $"No existe el empleado {entidad.ResponsableId.Value}.");
        var area = areas.Buscar(entidad.AreaId!.Value)
                   ?? throw new InvalidOperationException($"No existe el área {entidad.AreaId.Value}.");

        var motivo = string.Format(
            "El compromiso paso a ser {0} para la fecha {1} a cargo de {2} {3} que pertence al área {4}",
            verbo, fecha!.Value.ToString("dd/MM/yyyy"), empleado.Nombre, empleado.ApellidoPaterno,
            area.Descripcion);

        return motivo + "\nLa acción a realizar es: " + (entidad.Accion ?? "");
    }
}