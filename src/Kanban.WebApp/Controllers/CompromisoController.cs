using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Application.Abstractions.UseCases.Compromiso;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Domain;
using Kanban.Domain.Adicionales;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Compromisos;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.WebApp.Commons;
using Kanban.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class CompromisoController(
    IEstructuraLogica estructuraLogica,
    ICompromisoLogica compromisoLogica,
    IEmpleadoLogica empleadoLogica,
    IOrigenLogica origenLogica,
    IIndicadorLogica indicadorLogica,
    IPlanAccionLogica planAccionLogica,
    IEstructuraAreaLogica estructuraAreaLogica,
    IWebHostEnvironment entorno) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetCompromiso", int? tableroId = null)
    {
        try
        {
            var user = HttpContext.GetUser()!;

            Compromiso model = new Compromiso { EstructuraId = user.StructureId };
            if (tableroId.HasValue) model.TableroId = tableroId.Value;
            ViewBag.Tableros = user.Tables;
            ViewBag.Ruta = string.Concat("/ALICORP", estructuraLogica.Ruta(user.StructureId)?.ToUpper() ?? "");
            ViewBag.CallBack = callBack;

            ViewBag.Origenes = origenLogica.Listar(new OrigenFiltro { Activo = true });

            ViewBag.Indicadores = indicadorLogica.Listar(new IndicadorFiltro { Activo = true });

            ViewBag.Empleado = string.Format("{0}", user.Employee);

            return PartialView("_Nuevo", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult NuevoPorPlanAccion(int planAccionId, string callBack = "SetCompromiso", int? tableroId = null)
    {
        try
        {
            var user = HttpContext.GetUser()!;

            Compromiso model = new Compromiso { EstructuraId = user.StructureId, PlanAccionId = planAccionId };
            if (tableroId.HasValue) model.TableroId = tableroId.Value;

            ViewBag.Tableros = user.Tables;
            ViewBag.Ruta = string.Concat("/ALICORP", estructuraLogica.Ruta(user.StructureId)?.ToUpper() ?? "");
            ViewBag.CallBack = callBack;

            ViewBag.Origenes = origenLogica.Listar(new OrigenFiltro { Activo = true });

            ViewBag.Indicadores = indicadorLogica.Listar(new IndicadorFiltro { Activo = true });

            ViewBag.Empleado = string.Format("{0}", user.Employee);

            var accion = planAccionLogica.Buscar(planAccionId);

            model.Descripcion = accion.Descripcion;

            return PartialView("_Nuevo", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Nuevo(CompromisoModel.Nuevo model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;

            Compromiso entidad = model.Get();
            entidad.EstructuraId = user.StructureId;
            entidad.Estado = EstadoCompromiso.NUEVO;
            entidad.UsuarioRegistroId = user.UserId;
            entidad.EmpleadoRegistroId = user.EmployeeId;

            if (model.Foto != null) entidad.Foto = AlmacenArchivos.Guardar(model.Foto);

            compromisoLogica.Guardar(entidad, user.UserId, user.EmployeeId);

            return Content(entidad.Id.ToString());
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetCompromiso")
    {
        try
        {

            Compromiso model = compromisoLogica.Buscar(id);

            ViewBag.Tablero = string.Concat("/ALICORP", estructuraLogica.Ruta(model.TableroId)?.ToUpper() ?? "");
            ViewBag.CallBack = callBack;

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(CompromisoModel.Editar model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            Compromiso entidad = model.Get();

            compromisoLogica.Actualizar(entidad);
            return Content(model.Id.ToString());
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Seguimiento(int id)
    {
        try
        {

            Compromiso model = compromisoLogica.Buscar(id, true);
            ViewBag.Ruta = string.Concat("/ALICORP", estructuraLogica.Ruta(model.EstructuraId)?.ToUpper() ?? "");

            return PartialView("_Seguimiento", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Verificacion()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Verificacion(CompromisoModel.Verificar model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;
            Compromiso compromiso = new Compromiso
            {
                Id = model.Id.Value,
                Accion = model.Respuesta,
                Respuesta = model.Respuesta,
                FechaProgramacion = model.Fecha.Value,
                Estado = EstadoCompromiso.PROGRAMADO,
                AreaId = user.AreaId,
                ResponsableId = user.EmployeeId
            };
            compromisoLogica.AsignarAutomatico(compromiso, user.UserId, user.EmployeeId);
            return Content(model.Id.Value.ToString());
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Administracion()
    {
        return View();
    }

    [SafetyFilter]
    public ActionResult Tablero()
    {
        try
        {
            var user = HttpContext.GetUser()!;

            compromisoLogica.FueraFecha();
            List<TableroResumen> tableros = estructuraLogica.Listar(user.Tables.Select(x => x.Id).ToArray());
            
            return View(tableros);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [SafetyFilter]
    public ActionResult Gestion(int id)
    {
        try
        {

            var tablero = estructuraLogica.Buscar(id, true);

            CompromisoFiltro filtro = new CompromisoFiltro
            {
                TableroId = id,
                Estados = new [] {
                    (int)EstadoCompromiso.NUEVO,
                    (int)EstadoCompromiso.PENDIENTE,
                    (int)EstadoCompromiso.POR_VERIFICAR,
                    (int)EstadoCompromiso.FUERA_DE_FECHA,
                    (int)EstadoCompromiso.PROGRAMADO,
                    (int)EstadoCompromiso.REPROGRAMADO
                }
            };

            ViewBag.Compromisos = compromisoLogica.Listar(filtro) ?? new List<Compromiso>();

            return View(tablero);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    public ActionResult Ver(int id, bool parcial = true)
    {
        try
        {

            Compromiso model = compromisoLogica.Buscar(id);

            ViewBag.Tablero = string.Concat("/ALICORP", estructuraLogica.Ruta(model.TableroId)?.ToUpper() ?? "");
            ViewBag.Ruta = string.Concat("/ALICORP", estructuraLogica.Ruta(model.EstructuraId)?.ToUpper() ?? "");

            if (parcial) return PartialView("_Ver", model);
            else return View(model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Asignar(int id, string callBack)
    {
        try
        {

            ViewBag.CallBack = callBack;
            Compromiso model = compromisoLogica.Buscar(id, true);
            if (model != null)
            {
                model.Id = id;

                ViewBag.Ruta = string.Concat("/ALICORP", estructuraLogica.Ruta(model.EstructuraId)?.ToUpper() ?? "");
                ViewBag.Areas = (estructuraLogica.Listar(model.TableroId) ?? new List<EstructuraArea>()).Select(x => x.Area).ToList();
                ViewBag.Empleados = model.AreaId.HasValue ? empleadoLogica.Listar(new EmpleadoFiltro { AreaId = model.AreaId }) : new List<Empleado>();
            }

            return PartialView("_Asignar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Asignar(CompromisoModel.Asignar model)
    {
        try
        {

            Compromiso compromiso = compromisoLogica.Buscar(model.Id);
            Validar(compromiso, model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;

            compromiso.Id = model.Id;
            compromiso.AreaId = model.AreaId;
            compromiso.ResponsableId = model.ResponsableId;
            compromiso.Accion = model.Accion;
            if (model.PorVerificar == true) compromiso.Estado = EstadoCompromiso.POR_VERIFICAR;
            if (model.Finalizo == true) compromiso.Estado = EstadoCompromiso.FINALIZADO;

            compromisoLogica.Asignar(compromiso, user.UserId, user.EmployeeId);

            return Content(compromiso.Id.ToString());
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Asignado(string id)
    {
        var user = HttpContext.GetUser()!;
        ViewBag.ResponsableId = user.EmployeeId;
        ViewBag.Codigo = id;
        return View();
    }

    [HttpPost]
    public ActionResult Asignado(int id)
    {
        try
        {
            var user = HttpContext.GetUser()!;
            Compromiso compromiso = compromisoLogica.Buscar(id);
            compromiso.Estado = EstadoCompromiso.POR_VERIFICAR;
            compromisoLogica.CambiarEstado(compromiso, null, user.UserId, user.EmployeeId);
            return Content(compromiso.Id.ToString());
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Rechazar(CompromisoModel.Rechazar model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;
            var compromiso = compromisoLogica.Buscar(model.Id.Value);
            if (compromiso == null) return Validation("No se encontró el compromiso.");

            CompromisoFiltro filtro = new CompromisoFiltro { TableroId = compromiso.TableroId };

            if (compromiso.InstanciaId.HasValue) filtro.InstanciaId = compromiso.InstanciaId;
            else
            {
                filtro.Estado = compromiso.Estado;
                filtro.InstanciaId = 0;
            }

            compromiso.Estado = EstadoCompromiso.RECHAZADO;
            compromiso.Respuesta = model.Motivo;
            compromisoLogica.CambiarEstado(compromiso, null, user.UserId, user.EmployeeId);

            List<Compromiso> compromisos = compromisoLogica.Listar(filtro);
            string rpta = JsonConvert.SerializeObject(compromisos.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Descripcion,
                Estado = x.Estado.ToString(),
                x.InstanciaId,
                x.AreaId,
                FechaProgramacion = x.FechaProgramacion?.ToString("yyyy-MM-dd"),
                FechaReprogramacion = x.FechaReprogramacion?.ToString("yyyy-MM-dd"),
                Area = !x.AreaId.HasValue ? null : new
                {
                    x.Area.Descripcion,
                    ColorFondo = new
                    {
                        x.Area.ColorFondo.Clase,
                        x.Area.ColorFondo.Hex,
                        x.Area.ColorFondo.Rgba
                    }
                }
            }));

            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult PorVerificar(CompromisoModel.PorVerificar model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;
            var compromiso = compromisoLogica.Buscar(model.Id.Value);
            if (compromiso == null) return Validation("No se encontró el compromiso.");

            compromiso.Estado = EstadoCompromiso.POR_VERIFICAR;
            compromisoLogica.CambiarEstado(compromiso, null, user.UserId, user.EmployeeId);

            CompromisoFiltro filtro = new CompromisoFiltro
            {
                TableroId = compromiso.TableroId,
                Estado = EstadoCompromiso.POR_VERIFICAR,
                InstanciaId = 0
            };
            List<Compromiso> compromisos = compromisoLogica.Listar(filtro);
            string rpta = JsonConvert.SerializeObject(compromisos.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Descripcion,
                Estado = x.Estado.ToString(),
                x.InstanciaId,
                x.AreaId,
                FechaProgramacion = x.FechaProgramacion?.ToString("yyyy-MM-dd"),
                FechaReprogramacion = x.FechaReprogramacion?.ToString("yyyy-MM-dd"),
                Area = !x.AreaId.HasValue ? null : new
                {
                    x.Area.Descripcion,
                    ColorFondo = new
                    {
                        x.Area.ColorFondo.Clase,
                        x.Area.ColorFondo.Hex,
                        x.Area.ColorFondo.Rgba
                    }
                }
            }));

            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Finalizar(CompromisoModel.Finalizar model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;
            var compromiso = compromisoLogica.Buscar(model.Id.Value);
            if (compromiso == null) return Validation("No se encontró el compromiso.");

            compromiso.Estado = EstadoCompromiso.FINALIZADO;
            compromisoLogica.CambiarEstado(compromiso, null, user.UserId, user.EmployeeId);

            CompromisoFiltro filtro = new CompromisoFiltro
            {
                TableroId = compromiso.TableroId,
                Estado = EstadoCompromiso.POR_VERIFICAR,
                InstanciaId = 0
            };
            List<Compromiso> compromisos = compromisoLogica.Listar(filtro);
            string rpta = JsonConvert.SerializeObject(compromisos.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Descripcion,
                Estado = x.Estado.ToString(),
                x.InstanciaId,
                x.AreaId,
                FechaProgramacion = x.FechaProgramacion?.ToString("yyyy-MM-dd"),
                FechaReprogramacion = x.FechaReprogramacion?.ToString("yyyy-MM-dd"),
                Area = !x.AreaId.HasValue ? null : new
                {
                    x.Area.Descripcion,
                    ColorFondo = new
                    {
                        x.Area.ColorFondo.Clase,
                        x.Area.ColorFondo.Hex,
                        x.Area.ColorFondo.Rgba
                    }
                }
            }));
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult EscalarGerencia(CompromisoModel.EscalarGerencia model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;
            var compromiso = compromisoLogica.Buscar(model.Id.Value);
            if (compromiso == null) return Validation("No se encontró el compromiso.");


            compromisoLogica.CambiarInstancia(model.Id.Value, model.Motivo, (int)InstanciaObligatoria.GERENCIA, user.UserId, user.EmployeeId);

            CompromisoFiltro filtro = new CompromisoFiltro
            {
                TableroId = compromiso.TableroId,
                InstanciaId = (int)InstanciaObligatoria.GERENCIA
            };
            List<Compromiso> compromisos = compromisoLogica.Listar(filtro);
            string rpta = JsonConvert.SerializeObject(compromisos.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Descripcion,
                Estado = x.Estado.ToString(),
                x.InstanciaId,
                x.AreaId,
                FechaProgramacion = x.FechaProgramacion?.ToString("yyyy-MM-dd"),
                FechaReprogramacion = x.FechaReprogramacion?.ToString("yyyy-MM-dd"),
                Area = !x.AreaId.HasValue ? null : new
                {
                    x.Area.Descripcion,
                    ColorFondo = new
                    {
                        x.Area.ColorFondo.Clase,
                        x.Area.ColorFondo.Hex,
                        x.Area.ColorFondo.Rgba
                    }
                }
            }));
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }
    
    [HttpPost]
    public ActionResult CambiarEstado(CompromisoModel.CambiarEstado model)
    {
        try
        {
            var compromiso = compromisoLogica.Buscar(model.Id);
            if (compromiso == null) return Validation("No se encontró el compromiso.");

            if (model.Estado == EstadoCompromiso.PROGRAMADO) compromiso.FechaProgramacion = model.FechaProgramacion;
            if (model.Estado == EstadoCompromiso.REPROGRAMADO) compromiso.FechaReprogramacion = model.FechaReprogramacion;

            ValidarEstado(compromiso, model.Estado);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;
            EstadoCompromiso estadoOriginal = compromiso.Estado;

            compromiso.Id = model.Id;
            compromiso.Respuesta = model.Respuesta;
            compromiso.Estado = model.Estado;

            compromisoLogica.CambiarEstado(compromiso, model.Motivo, user.UserId, user.EmployeeId);
            return Content("Se cambió de estado");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult CambiarInstancia(int id, string motivo, int instanciaId)
    {
        try
        {
            Compromiso compromiso = compromisoLogica.Buscar(id);
            ValidarInstancia(compromiso, instanciaId);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;
            compromisoLogica.CambiarInstancia(id, motivo, instanciaId, user.UserId, user.EmployeeId);
            return Content("Se cambió de instancia.");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult ListarPorEstado(int tableroId, EstadoCompromiso estado)
    {
        try
        {
            CompromisoFiltro filtro = new CompromisoFiltro { TableroId = tableroId, Estado = estado, InstanciaId = 0 };
            List<Compromiso> compromisos = compromisoLogica.Listar(filtro);
            string rpta = ObtenerCompromisoJson(compromisos);
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult ListarPorInstancia(int tableroId, int instanciaId)
    {
        try
        {
            CompromisoFiltro filtro = new CompromisoFiltro { TableroId = tableroId, InstanciaId = instanciaId };
            List<Compromiso> compromisos = compromisoLogica.Listar(filtro);
            string rpta = ObtenerCompromisoJson(compromisos);
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar(CompromisoFiltro filtro)
    {
        try
        {
            List<Compromiso> compromisos = compromisoLogica.Listar(filtro);
            string rpta = JsonConvert.SerializeObject(compromisos.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Descripcion,
                Estado = x.Estado.ToString(),
                x.InstanciaId,
                x.AreaId,
                FechaProgramacion = x.FechaProgramacion?.ToString("yyyy-MM-dd"),
                FechaReprogramacion = x.FechaReprogramacion?.ToString("yyyy-MM-dd"),
                Area = !x.AreaId.HasValue ? null : new
                {
                    x.Area.Descripcion,
                    ColorFondo = new
                    {
                        x.Area.ColorFondo.Clase,
                        x.Area.ColorFondo.Hex,
                        x.Area.ColorFondo.Rgba
                    }
                }
            }));

            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Indicador()
    {
        try
        {
            var user = HttpContext.GetUser()!;
            return View(user.Tables);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [HttpPost]
    public ActionResult Indicador(CompromisoModel.Indicador model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            List<Indicador> indicadores = indicadorLogica.Listar(new IndicadorFiltro { Activo = true });
            List<Area> areas = estructuraAreaLogica.Listar(model.TableroId)?.Select(x => x.Area).ToList();

            var indicador = compromisoLogica.Indicador(model.TableroId, model.FechaDesde, model.FechaHasta);
            var indicadorPorEstado_1_1 = indicador.PorEstado11;
            var indicadorPorEstado_1_2 = indicador.PorEstado12;
            var compromisosPorTablero = indicador.PorTablero;

            List<int> empleadoIds = indicadorPorEstado_1_1.Where(x => x.ResponsableId.HasValue).Select(x => x.ResponsableId.Value).ToList();
            if (indicadorPorEstado_1_2.Any(x => x.ResponsableId.HasValue))
                empleadoIds.AddRange(indicadorPorEstado_1_2.Where(x => x.ResponsableId.HasValue).Select(x => x.ResponsableId.Value).ToList());

            if (empleadoIds.Count == 0) empleadoIds.Add(0);

            List<Empleado> empleados = empleadoLogica.Listar(new EmpleadoFiltro { Ids = empleadoIds.ToArray() });

            if (compromisosPorTablero != null && compromisosPorTablero.Count > 0)
                empleadoIds = compromisosPorTablero.Select(x => x.EmpleadoRegistroId).ToList();
            else empleadoIds = new List<int>() { 0 };

            List<Empleado> empleadosPorTablero = empleadoLogica.Listar(new EmpleadoFiltro { Ids = empleadoIds.ToArray() });

            string rpta = JsonConvert.SerializeObject(new
            {
                Indicadores = indicadores,
                Areas = areas,
                Empleados = empleados.Select(x => new { x.Id, x.Nombre, x.ApellidoPaterno, x.ApellidoMaterno }),
                IndicadorPorEstado_1_1 = indicadorPorEstado_1_1.Select(x => new { x.Id, x.Impacto, x.Estado, x.AreaId, x.ResponsableId }),
                IndicadorPorEstado_1_2 = indicadorPorEstado_1_2.Select(x => new { x.Id, x.Impacto, x.Estado, x.AreaId, x.ResponsableId }),

                EmpleadoPorTablero = empleadosPorTablero.Select(x => new { x.Id, x.Nombre, x.ApellidoPaterno, x.ApellidoMaterno }),
                CompromisoPorTablero = compromisosPorTablero.Select(x => new { x.Id, x.EmpleadoRegistroId, x.Estado, x.InstanciaId })
            });

            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }

    }

    [HttpPost]
    public ActionResult Reprogramar(CompromisoModel.Reprogramar model)
    {
        try
        {
            Compromiso compromiso = compromisoLogica.Buscar(model.Id);
            Validar(model);
            if (ModelState.IsValid)
            {
                compromiso.FechaReprogramacion = model.FechaReprogramacion;
                ValidarEstado(compromiso, EstadoCompromiso.REPROGRAMADO);
            }
            if (!ModelState.IsValid) return Validation();
            var user = HttpContext.GetUser()!;
            EstadoCompromiso estadoOriginal = compromiso.Estado;

            compromiso.Id = model.Id;
            compromiso.Respuesta = "";
            compromiso.Estado = EstadoCompromiso.REPROGRAMADO;

            compromisoLogica.CambiarEstado(compromiso, null, user.UserId, user.EmployeeId);
            return Content("Se cambió de estado");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult ListarPorPagina(CompromisoFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            var usuario = HttpContext.GetUser()!;

            if (filter == null) filter = new CompromisoFiltro { Estructuras = usuario.StructuresId };
            else filter.Estructuras = usuario.StructuresId;

            var paginado = compromisoLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;

            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Codigo,
                    x.Descripcion,
                    Fecha = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    Estado = x.Estado.ToString(),
                    Nuevo = x.FechaRegistro.Date == DateTime.Today,
                    Tablero = new { x.Tablero.Descripcion },
                    ColorFondo = new
                    {
                        x.Area?.ColorFondo.Clase,
                        x.Area?.ColorFondo.Hex,
                        x.Area?.ColorFondo.Rgba,
                    },
                    ColorTexto = new
                    {
                        x.Area?.ColorTexto.Clase,
                        x.Area?.ColorTexto.Hex,
                        x.Area?.ColorTexto.Rgba,
                    },
                    x.AreaId,
                    Area = new { x.Area?.Descripcion },
                    x.InstanciaId,
                    Instancia = new { x.Instancia?.Abreviatura },
                    FechaProgramacion = x.FechaProgramacion?.ToString("dd/MM/yyyy"),
                    FechaReprogramacion = x.FechaReprogramacion?.ToString("dd/MM/yyyy"),
                    Estructura = new { x.Estructura.Descripcion }
                }),
                totalRows
            });
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
            //Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //ViewBag.Message = ex.Message;
            //return PartialView("_Error");
        }
    }

    public ActionResult ListarAsignadoPorPagina(CompromisoFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            if (filter == null) filter = new CompromisoFiltro();
            var user = HttpContext.GetUser()!;

            filter.Estados = new int[] { (int)EstadoCompromiso.PROGRAMADO, (int)EstadoCompromiso.REPROGRAMADO };
            filter.ResponsableId = user.EmployeeId;
            filter.InstanciaId = 0;

            var paginado = compromisoLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;

            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Codigo,
                    x.Descripcion,
                    Fecha = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    Estado = x.Estado.ToString(),
                    Nuevo = x.FechaRegistro.Date == DateTime.Today,
                    Tablero = new { x.Tablero.Descripcion },
                    ColorFondo = new
                    {
                        x.Area?.ColorFondo.Clase,
                        x.Area?.ColorFondo.Hex,
                        x.Area?.ColorFondo.Rgba,
                    },
                    ColorTexto = new
                    {
                        x.Area?.ColorTexto.Clase,
                        x.Area?.ColorTexto.Hex,
                        x.Area?.ColorTexto.Rgba,
                    },
                    x.AreaId,
                    Area = new { x.Area?.Descripcion },
                    x.InstanciaId,
                    Instancia = new { x.Instancia?.Abreviatura },
                    FechaProgramacion = x.FechaProgramacion?.ToString("dd/MM/yyyy"),
                    FechaReprogramacion = x.FechaReprogramacion?.ToString("dd/MM/yyyy"),
                    Estructura = new { x.Estructura.Descripcion }
                }),
                totalRows
            });
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);

            //Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //ViewBag.Message = ex.Message;
            //return PartialView("_Error");
        }
    }

    [HttpPost]
    public ActionResult ExportarExcel(CompromisoModel.ExportarExcel model)
    {
        try
        {
            ExcelPackage paquete = new ExcelPackage(new FileInfo(Path.Combine(entorno.WebRootPath, "reports", "rptCompromiso.xlsx")));
            ExcelWorksheet hojaCompromiso = paquete.Workbook.Worksheets[1],
                hojaEstado = paquete.Workbook.Worksheets[2],
                hojaInstancia = paquete.Workbook.Worksheets[3];


            //compromisoLogica.Exportar puede devolver una lista vacía.
            var exportacion = compromisoLogica.Exportar(new CompromisoFiltro
            {
                TableroId = model.TableroId,
                FechaRegistroDesde = model.Desde.ToString("dd/MM/yyyy"),
                FechaRegistroHasta = model.Hasta.ToString("dd/MM/yyyy")
            });
            var compromisos = exportacion.Compromisos;
            var estados = exportacion.Estados;
            var instancias = exportacion.Instancias;

            #region Compromisos

            int fila = 3;
            foreach (var item in compromisos.OrderBy(x => x.FechaRegistro))
            {
                hojaCompromiso.Cells[fila, 1].Value = fila - 2;
                hojaCompromiso.Cells[fila, 2].Value = item.Id;
                hojaCompromiso.Cells[fila, 3].Value = item.Codigo;
                hojaCompromiso.Cells[fila, 4].Value = item.UsuarioRegistro.Nombre;
                hojaCompromiso.Cells[fila, 5].Value = string.Format("{0} {1}, {2}", item.EmpleadoRegistro.ApellidoPaterno.ToUpper(), item.EmpleadoRegistro.ApellidoMaterno.ToUpper(), item.EmpleadoRegistro.Nombre.ToUpper());
                hojaCompromiso.Cells[fila, 6].Value = item.Estructura.Descripcion;
                hojaCompromiso.Cells[fila, 7].Value = item.Tablero.Descripcion;
                hojaCompromiso.Cells[fila, 8].Value = item.Descripcion;
                hojaCompromiso.Cells[fila, 9].Value = item.FechaRegistro.ToString("dd/MM/yyyy HH:mm");
                hojaCompromiso.Cells[fila, 10].Value = item.Origen;
                hojaCompromiso.Cells[fila, 11].Value = item.Impacto;
                hojaCompromiso.Cells[fila, 12].Value = item.FotoId.HasValue ? "SI" : "NO";
                hojaCompromiso.Cells[fila, 13].Value = item.Detalle;
                hojaCompromiso.Cells[fila, 14].Value = item.Instancia == null ? item.Estado.ToString() : "";
                hojaCompromiso.Cells[fila, 15].Value = item.Instancia?.Descripcion ?? "";
                hojaCompromiso.Cells[fila, 16].Value = item.FechaProgramacion?.ToString("dd/MM/yyyy") ?? "";
                hojaCompromiso.Cells[fila, 17].Value = item.FechaReprogramacion?.ToString("dd/MM/yyyy") ?? "";

                hojaCompromiso.Cells[fila, 18].Value = item.Area?.Descripcion ?? "";
                hojaCompromiso.Cells[fila, 19].Value = item.Responsable != null ? string.Format("{0} {1}, {2}", item.Responsable.ApellidoPaterno.ToUpper(), item.Responsable.ApellidoMaterno.ToUpper(), item.Responsable.Nombre.ToUpper()) : "";
                hojaCompromiso.Cells[fila, 20].Value = item.Accion ?? "";

                fila++;
            }

            if (fila > 3)
            {
                var cells = hojaCompromiso.Cells["A3:R" + (fila - 1)];
                cells.Style.Border.Top.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Left.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Hair;

                cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);
            }

            #endregion

            #region Estados

            fila = 2;
            foreach (var item in estados.OrderBy(x => x.CompromisoId).ThenBy(x => x.FechaRegistro))
            {
                hojaEstado.Cells[fila, 1].Value = fila - 1;
                hojaEstado.Cells[fila, 2].Value = item.CompromisoId;
                hojaEstado.Cells[fila, 3].Value = item.Estado.ToString().Replace("_", " ");
                hojaEstado.Cells[fila, 4].Value = item.FechaRegistro.ToString("dd/MM/yyyy HH:mm");
                
                if (item.Estado == EstadoCompromiso.RECHAZADO)
                    hojaEstado.Cells[fila, 5].Value = compromisos.FirstOrDefault(x => x.Id == item.CompromisoId)?.Respuesta ?? "";
                else
                    hojaEstado.Cells[fila, 5].Value = item.Motivo;

                hojaEstado.Cells[fila, 6].Value = item.Usuario?.Nombre ?? "";

                if(item.Empleado != null)
                    hojaEstado.Cells[fila, 7].Value = string.Format("{0} {1}, {2}", item.Empleado.ApellidoPaterno.ToUpper(), item.Empleado.ApellidoMaterno.ToUpper(), item.Empleado.Nombre.ToUpper());

                fila++;
            }

            if (fila > 2)
            {
                var cells = hojaEstado.Cells["A2:G" + (fila - 1)];
                cells.Style.Border.Top.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Left.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Hair;

                cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);
            }

            #endregion

            #region Instancias

            fila = 2;
            foreach (var item in instancias.OrderBy(x => x.CompromisoId).ThenBy(x => x.FechaRegistro))
            {
                hojaInstancia.Cells[fila, 1].Value = fila - 1;
                hojaInstancia.Cells[fila, 2].Value = item.CompromisoId;
                hojaInstancia.Cells[fila, 3].Value = item.Instancia.Descripcion;
                hojaInstancia.Cells[fila, 4].Value = item.FechaRegistro.ToString("dd/MM/yyyy HH:mm");
                hojaInstancia.Cells[fila, 5].Value = item.Motivo;
                hojaInstancia.Cells[fila, 6].Value = item.Usuario?.Nombre ?? "";
                if (item.Empleado != null)
                    hojaInstancia.Cells[fila, 7].Value = string.Format("{0} {1}, {2}", item.Empleado.ApellidoPaterno.ToUpper(), item.Empleado.ApellidoMaterno.ToUpper(), item.Empleado.Nombre.ToUpper());
            }

            if (fila > 2)
            {
                var cells = hojaInstancia.Cells["A2:G" + (fila - 1)];
                cells.Style.Border.Top.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Left.Style = ExcelBorderStyle.Hair;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Hair;

                cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Gray);
                cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Gray);
            }

            #endregion

            byte[] data = paquete.GetAsByteArray();

            hojaCompromiso.Dispose();
            hojaEstado.Dispose();
            hojaInstancia.Dispose();
            paquete.Dispose();

            return File(data, "application/octet-stream", string.Concat("Compromisos_", DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".xlsx"));
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
            //Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //ViewBag.Message = ex.Message;
            //return PartialView("_Error");
        }
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void Validar(CompromisoModel.Editar model)
    {
        ModelState.Clear();
        if (!model.Id.HasValue || model.Id.Value <= 0) ModelState.AddModelError("Id", "El compromiso no tiene un identificador.");
        if (string.IsNullOrWhiteSpace(model.Descripcion)) ModelState.AddModelError("Descripcion", "Es necesario ingresar una breve descripción.");
        else if (model.Descripcion.Length > 30) ModelState.AddModelError("Descripcion", "La breve descripción solo puede contar con 30 caracteres.");
    }

    [NonAction]
    private void Validar(CompromisoModel.Nuevo model)
    {
        ModelState.Clear();
        if (!model.TableroId.HasValue || model.TableroId.Value <= 0) ModelState.AddModelError("TableroId", "Es necesario seleccionar el tablero.");
        if (string.IsNullOrWhiteSpace(model.Descripcion)) ModelState.AddModelError("Descripcion", "Es necesario ingresar una breve descripción.");
        else if (model.Descripcion.Length > 30) ModelState.AddModelError("Descripcion", "La breve descripción solo puede contar con 30 caracteres.");
    }

    [NonAction]
    private void Validar(CompromisoModel.Verificar model)
    {
        ModelState.Clear();
        if (!model.Id.HasValue || model.Id.Value <= 0) ModelState.AddModelError("Id", "El compromiso no tiene un identificador.");
        if (string.IsNullOrWhiteSpace(model.Respuesta)) ModelState.AddModelError("Respuesta", "Es necesario ingresar respuesta.");
        if (!model.Fecha.HasValue || model.Fecha.Value == DateTime.MinValue) ModelState.AddModelError("Fecha", "Es necesario ingresar fecha válida.");
        else if (model.Fecha.Value < DateTime.Today) ModelState.AddModelError("Fecha", "La fecha no debe ser menor al dia de hoy.");
    }

    [NonAction]
    private void ValidarEstado(Compromiso model, EstadoCompromiso estado)
    {
        ModelState.Clear();
        if (!model.InstanciaId.HasValue)
        {
            switch (model.Estado)
            {
                case EstadoCompromiso.NUEVO:
                    if (estado != EstadoCompromiso.PENDIENTE && estado != EstadoCompromiso.RECHAZADO && estado != EstadoCompromiso.PROGRAMADO) ModelState.AddModelError("Estado", string.Concat("No se puede mover del estado NUEVO a ", estado.ToString().Replace("_", " ")));
                    else if (estado == EstadoCompromiso.PROGRAMADO)
                    {
                        if (!model.AreaId.HasValue) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga un área responsable.");
                        if (!model.ResponsableId.HasValue) ModelState.AddModelError("EmpleadoId", "Es necesario que el compromiso tenga un responsable.");
                        if (string.IsNullOrEmpty(model.Accion)) ModelState.AddModelError("Accion", "Es necesario ingresar la accion a realizar.");
                    }
                    break;
                case EstadoCompromiso.PENDIENTE:
                    if (estado != EstadoCompromiso.PROGRAMADO && estado != EstadoCompromiso.RECHAZADO) ModelState.AddModelError("Estado", string.Concat("No se puede mover del estado PENDIENTE a ", estado.ToString().Replace("_", " ")));
                    else if (estado == EstadoCompromiso.PROGRAMADO)
                    {
                        if (!model.AreaId.HasValue) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga un área responsable.");
                        if (!model.ResponsableId.HasValue) ModelState.AddModelError("EmpleadoId", "Es necesario que el compromiso tenga un responsable.");
                        if (string.IsNullOrEmpty(model.Accion)) ModelState.AddModelError("Accion", "Es necesario ingresar la accion a realizar.");
                    }
                    break;
                case EstadoCompromiso.FUERA_DE_FECHA:
                    if (estado != EstadoCompromiso.REPROGRAMADO) ModelState.AddModelError("Estado", string.Concat("No se puede mover del estado PROGRAMADO a ", estado.ToString().Replace("_", " ")));
                    break;
                case EstadoCompromiso.PROGRAMADO:
                    if (estado != EstadoCompromiso.REPROGRAMADO && estado != EstadoCompromiso.POR_VERIFICAR) ModelState.AddModelError("Estado", string.Concat("No se puede mover del estado PROGRAMADO a ", estado.ToString().Replace("_", " ")));
                    if (!model.AreaId.HasValue) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga un área responsable.");
                    if (!model.ResponsableId.HasValue) ModelState.AddModelError("EmpleadoId", "Es necesario que el compromiso tenga un responsable.");
                    if (string.IsNullOrEmpty(model.Accion)) ModelState.AddModelError("Accion", "Es necesario ingresar una accion a ejecutar.");
                    break;
                case EstadoCompromiso.REPROGRAMADO:
                    if (estado != EstadoCompromiso.POR_VERIFICAR) ModelState.AddModelError("Estado", string.Concat("No se puede mover del estado PROGRAMADO a ", estado.ToString().Replace("_", " ")));
                    if (!model.AreaId.HasValue) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga un área responsable.");
                    if (!model.ResponsableId.HasValue) ModelState.AddModelError("EmpleadoId", "Es necesario que el compromiso tenga un responsable.");
                    if (string.IsNullOrEmpty(model.Accion)) ModelState.AddModelError("Accion", "Es necesario ingresar una accion a ejecutar.");
                    break;
                case EstadoCompromiso.POR_VERIFICAR:
                    if (estado != EstadoCompromiso.FINALIZADO && estado != EstadoCompromiso.PENDIENTE) ModelState.AddModelError("Estado", string.Concat("No se puede mover del estado POR_VERIFICAR a ", estado.ToString().Replace("_", " ")));
                    break;
                case EstadoCompromiso.FINALIZADO:
                    if (!string.IsNullOrEmpty(model.Respuesta)) ModelState.AddModelError("Estado", "Es necesario agregar una respuesta para FINALIZAR el compromiso.");
                    break;
                case EstadoCompromiso.RECHAZADO:
                    if (!string.IsNullOrEmpty(model.Respuesta)) ModelState.AddModelError("Estado", "Es necesario agregar una respuesta para RECHAZAR el compromiso.");
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (model.InstanciaId == (int)InstanciaObligatoria.DIRECCION || model.InstanciaId == (int)InstanciaObligatoria.GERENCIA)
            {
                switch (estado)
                {
                    case EstadoCompromiso.PROGRAMADO:
                    case EstadoCompromiso.REPROGRAMADO:
                        if (!model.AreaId.HasValue) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga un área responsable.");
                        if (!model.ResponsableId.HasValue) ModelState.AddModelError("EmpleadoId", "Es necesario que el compromiso tenga un responsable.");
                        if (string.IsNullOrEmpty(model.Accion)) ModelState.AddModelError("Accion", "Es necesario ingresar una accion a ejecutar.");
                        break;
                    case EstadoCompromiso.NUEVO:
                    case EstadoCompromiso.POR_VERIFICAR:
                    case EstadoCompromiso.FINALIZADO:
                        ModelState.AddModelError("Estado", string.Concat("No se puede mover el compromiso al estado ", estado.ToString().Replace("_", " ")));
                        break;
                }
                if (estado != EstadoCompromiso.PENDIENTE && estado == EstadoCompromiso.RECHAZADO) ModelState.AddModelError("Estado", string.Concat("No se puede mover el compromiso al estado ", estado.ToString().Replace("_", " ")));
            }
            else
            {
                if (estado != EstadoCompromiso.PROGRAMADO) ModelState.AddModelError("Estado", string.Concat("No se puede mover el compromiso al estado ", estado.ToString().Replace("_", " ")));
                if (!model.AreaId.HasValue) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga un área responsable.");
                if (!model.ResponsableId.HasValue) ModelState.AddModelError("EmpleadoId", "Es necesario que el compromiso tenga un responsable.");
            }
        }
    }

    [NonAction]
    private void ValidarInstancia(Compromiso model, int instanciaId)
    {
        ModelState.Clear();
        switch (instanciaId)
        {
            case (int)InstanciaObligatoria.GERENCIA:
                break;
            case (int)InstanciaObligatoria.DIRECCION:
                if (!model.InstanciaId.HasValue ? model.Estado != EstadoCompromiso.POR_VERIFICAR && model.Estado != EstadoCompromiso.FUERA_DE_FECHA : model.InstanciaId != (int)InstanciaObligatoria.GERENCIA)
                    ModelState.AddModelError("InstanciaId", "No puede escalar a DIRECCION sino ha pasado por GERENCIA.");
                break;
            default:
                //if (!model.AreaId.HasValue) ModelState.AddModelError("AreaId", "No se puede escalar por que el compromiso no tiene un área responsable.");
                break;
        }
    }

    [NonAction]
    private void Validar(Compromiso model, CompromisoModel.Asignar asignar)
    {
        ModelState.Clear();
        switch (model.Estado)
        {
            case EstadoCompromiso.PROGRAMADO:
            case EstadoCompromiso.REPROGRAMADO:
                if (!model.AreaId.HasValue || model.AreaId <= 0) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga una área responsable.");
                if (!model.ResponsableId.HasValue || model.ResponsableId <= 0) ModelState.AddModelError("ResponsableId", "Es necesario que el compromiso tenga un responsable.");
                if (string.IsNullOrEmpty(asignar.Accion)) ModelState.AddModelError("Accion", "Es necesario ingresar la accion a realizar.");
                break;
            case EstadoCompromiso.POR_VERIFICAR:
                if (!model.AreaId.HasValue || model.AreaId <= 0) ModelState.AddModelError("AreaId", "Es necesario que el compromiso tenga una área responsable.");
                if (!model.ResponsableId.HasValue || model.ResponsableId <= 0) ModelState.AddModelError("ResponsableId", "Es necesario que el compromiso tenga un responsable.");
                break;
            case EstadoCompromiso.FINALIZADO:
                break;
            case EstadoCompromiso.RECHAZADO:
                break;
            default:
                break;
        }
    }

    [NonAction]
    private void Validar(CompromisoModel.Rechazar model)
    {
        ModelState.Clear();
        if (!model.Id.HasValue || model.Id.Value <= 0) ModelState.AddModelError("Id", "El compromiso no tiene un identificador.");
        if (string.IsNullOrWhiteSpace(model.Motivo)) ModelState.AddModelError("Motivo", "Es necesario ingresar un motivo.");
    }

    [NonAction]
    private void Validar(CompromisoModel.PorVerificar model)
    {
        ModelState.Clear();
        if (!model.Id.HasValue || model.Id.Value <= 0) ModelState.AddModelError("Id", "El compromiso no tiene un identificador.");
    }

    [NonAction]
    private void Validar(CompromisoModel.Finalizar model)
    {
        ModelState.Clear();
        if (!model.Id.HasValue || model.Id.Value <= 0) ModelState.AddModelError("Id", "El compromiso no tiene un identificador.");
    }

    [NonAction]
    private void Validar(CompromisoModel.EscalarGerencia model)
    {
        ModelState.Clear();
        if (!model.Id.HasValue || model.Id.Value <= 0) ModelState.AddModelError("Id", "El compromiso no tiene un identificador.");
        if (string.IsNullOrWhiteSpace(model.Motivo)) ModelState.AddModelError("Motivo", "Es necesario ingresar un motivo.");
    }

    [NonAction]
    private void Validar(CompromisoModel.Indicador model)
    {
        ModelState.Clear();
        if (model.TableroId <= 0) ModelState.AddModelError("TableroId", "Seleccione un tablero.");
        if (model.Anio <= 0) ModelState.AddModelError("Anio", "Seleccione un año.");
        if (model.Mes <= 0) ModelState.AddModelError("Anio", "Seleccione un mes.");
    }

    [NonAction]
    private void Validar(CompromisoModel.Reprogramar model)
    {
        ModelState.Clear();
        if (model.Id <= 0) ModelState.AddModelError("Id", "El compromiso no tiene un identificador.");
        if (!model.FechaReprogramacion.HasValue || model.FechaReprogramacion < DateTime.Today) ModelState.AddModelError("FechaReprogramacion", "La fecha de reprogramacion debe ser mayor o igual a hoy.");
    }

    [NonAction]
    private string ObtenerCompromisoJson(List<Compromiso> compromisos)
    {
        return JsonConvert.SerializeObject(compromisos.Select(x => new
        {
            x.Id,
            x.Codigo,
            x.Descripcion,
            Estado = x.Estado.ToString(),
            x.InstanciaId,
            x.AreaId,
            Area = !x.AreaId.HasValue ? null : new
            {
                x.Area.Descripcion,
                ColorFondo = new
                {
                    x.Area.ColorFondo.Clase,
                    x.Area.ColorFondo.Hex,
                    x.Area.ColorFondo.Rgba
                }
            },
            FechaProgramacion = x.FechaProgramacion?.ToString("yyyy-MM-dd"),
            FechaReprogramacion = x.FechaReprogramacion?.ToString("yyyy-MM-dd")
        }));
    }

    #endregion
}
