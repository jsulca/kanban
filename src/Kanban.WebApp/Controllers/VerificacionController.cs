using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.WebApp.Commons;
using Kanban.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Kanban.WebApp.Controllers;

public class VerificacionController(
    IVerificacionLogica verificacionLogica,
    IVerificarLogica verificarLogica,
    IEstructuraLogica estructuraLogica,
    ITipoVerificacionLogica tipoVerificacionLogica,
    IEstructuraEmpleadoLogica estructuraEmpleadoLogica,
    ISostenibilidadLogica sostenibilidadLogica,
    IWebHostEnvironment entorno) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        var user = HttpContext.GetUser()!;
        ViewBag.EmpleadoId = user.EmployeeId;
        return View();
    }

    [SafetyFilter]
    public ActionResult Modelar()
    {
        return View();
    }

    [SafetyFilter]
    public ActionResult Verificar(int id)
    {
        try
        {
            var user = HttpContext.GetUser()!;

            ViewBag.Tableros = user.Tables;
            ViewBag.Empleado = user.Employee;

            Verificacion verificacion = verificacionLogica.Buscar(id, true);

            List<Estructura> estructuras = estructuraLogica.Arbol(user.Tables[0].Id);
            ViewBag.Estructuras = estructuras ?? new List<Estructura>();

            return View(verificacion);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [SafetyFilter]
    [HttpPost]
    public ActionResult Verificar(Verificar model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            var user = HttpContext.GetUser()!;

            model.UsuarioId = user.UserId;
            model.EmpleadoId = user.EmployeeId;

            List<EstructuraEmpleado> confirmadores = estructuraEmpleadoLogica.Listar(model.TableroId);
            List<Sostenibilidad> confirmadoresSostenibilidad = sostenibilidadLogica.Listar(model.TableroId);

            if (!confirmadores.Any(x => x.EmpleadoId == user.EmployeeId) && !confirmadoresSostenibilidad.Any(x => x.EmpleadoId == user.EmployeeId))
            {
                ModelState.AddModelError("EmpleadoId", "Usted no se encuentra registrado como confirmador del tablero seleccionado.");
                return Validation();
            }
            else
            {
                EstructuraEmpleado confirmador = confirmadores.FirstOrDefault(x => x.EmpleadoId == user.EmployeeId);
                //Agregar el area asociada
                model.AreaId = confirmador?.AreaId;

                //verificarLogica.Guardar(model);
                verificarLogica.GuardarEF(model);
            }

            return Content(model.Id.ToString());
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Todo()
    {
        return View();
    }

    [SafetyFilter]
    public ActionResult Nuevo(int id = 0)
    {
        try
        {
            Verificacion model = new Verificacion { Activo = true, Rom = true };
            if (id > 0)
            {
                model = verificacionLogica.Buscar(id, true);
            }

            ViewBag.TiposVerificacion = tipoVerificacionLogica.Listar(new TipoVerificacionFiltro { Activo = true });

            return View(model);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [HttpPost]
    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult Nuevo(Verificacion model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            verificacionLogica.Guardar(model);
            return Content(model.Id.ToString());

        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Editar(int id)
    {
        try
        {

            Verificacion model = verificacionLogica.Buscar(id, true);
            ViewBag.TiposVerificacion = tipoVerificacionLogica.Listar(new TipoVerificacionFiltro { Activo = true });

            return View(model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult Editar(Verificacion model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            verificacionLogica.Actualizar(model);
            return Content(model.Id.ToString());
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Tablero()
    {
        try
        {
            var user = HttpContext.GetUser()!;
            List<Estructura> tableros = user.Tables;
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

            Estructura model = estructuraLogica.Buscar(id);
            List<EstructuraEmpleado> empleados = estructuraEmpleadoLogica.Listar(id);
            List<Sostenibilidad> sostenibilidades = sostenibilidadLogica.Listar(id);

            ViewBag.Empleados = empleados;
            ViewBag.Sostenibilidades = sostenibilidades;
            ViewBag.TiposVerificacion = tipoVerificacionLogica.Listar(new TipoVerificacionFiltro { Activo = true });

            DateTime hoy = DateTime.Today;

            VerificarFiltro filtro = new VerificarFiltro
            {
                Desde = new DateTime(hoy.Year, hoy.Month, 1),
                Hasta = new DateTime(hoy.Year, hoy.Month, DateTime.DaysInMonth(hoy.Year, hoy.Month)),
                EmpleadoIds = empleados.Select(x => x.EmpleadoId).ToArray(),
                TableroId = id
            };

            ViewBag.Verificaciones = verificarLogica.TableroResumen(filtro);

            filtro = new VerificarFiltro
            {
                Desde = new DateTime(hoy.Year, 1, 1),
                Hasta = new DateTime(hoy.Year, 12, DateTime.DaysInMonth(hoy.Year, hoy.Month)),
                EmpleadoIds = sostenibilidades.Select(x => x.EmpleadoId).ToArray(),
                TableroId = id
            };

            ViewBag.VerificacionesSostenibilidad = verificarLogica.TableroResumen(filtro);

            var detalle = verificarLogica.Listar(id, hoy.Year, hoy.Month);
            var semanas = detalle.Semanas;
            var comentarios = detalle.Comentarios;
            var meses = detalle.Meses;

            ViewBag.ConfirmadorSemana = semanas;
            ViewBag.ConfirmadorComentario = comentarios;
            ViewBag.SostenibilidadMes = meses;

            return View(model);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [SafetyFilter]
    public ActionResult Ver(int id)
    {
        try
        {
            Verificar model = verificarLogica.Buscar(id, true);
            return View(model);
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult ListarPorPagina(VerificacionFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            var paginado = verificacionLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;

            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Nombre,
                    x.Activo,
                    x.Rom,
                    x.TipoVerificacionId,
                    TipoVerificacion = new { x.TipoVerificacion.Nombre }
                }),
                totalRows
            });
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    //TODO: AGREGAR FILTRO POR TABLERO Y POR CELULAR
    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult ListarVerificarPorPagina(VerificarFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            var paginado = verificarLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;

            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Encargado,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    x.PuntajeMaximo,
                    x.PuntajeObtenido,
                    Verificacion = new { x.Verificacion.Nombre },
                    Empleado = new { x.Empleado.Nombre, x.Empleado.ApellidoPaterno, x.Empleado.ApellidoMaterno },
                    x.EstructuraId,
                    Estructura = new { x.Estructura.Descripcion },
                    x.TableroId,
                    Tablero = new { x.Tablero.Descripcion },
                    x.VP
                }),
                totalRows
            });
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult Seleccionar(string callBack = "SetVerificacion")
    {
        ViewBag.CallBack = callBack;
        return PartialView("_Seleccionar");
    }

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult ListarResumen(int tableroId, int mes, int anio)
    {
        try
        {

            List<EstructuraEmpleado> empleados = estructuraEmpleadoLogica.Listar(tableroId);
            List<Sostenibilidad> sostenibilidades = sostenibilidadLogica.Listar(tableroId);

            DateTime hoy = new DateTime(anio, mes, 1);
            VerificarFiltro filtro = new VerificarFiltro
            {
                Desde = new DateTime(hoy.Year, hoy.Month, 1),
                Hasta = new DateTime(hoy.Year, hoy.Month, DateTime.DaysInMonth(hoy.Year, hoy.Month)),
                EmpleadoIds = empleados.Select(x => x.EmpleadoId).ToArray(),
                TableroId = tableroId
            };
            List<Verificar> verificaciones = verificarLogica.TableroResumen(filtro);

            filtro = new VerificarFiltro
            {
                Desde = new DateTime(hoy.Year, 1, 1),
                Hasta = new DateTime(hoy.Year, 12, DateTime.DaysInMonth(hoy.Year, hoy.Month)),
                EmpleadoIds = sostenibilidades.Select(x => x.EmpleadoId).ToArray(),
                TableroId = tableroId
            };

            var detalle = verificarLogica.Listar(tableroId, hoy.Year, hoy.Month);
            var semanas = detalle.Semanas;
            var comentarios = detalle.Comentarios;
            var meses = detalle.Meses;

            List<Verificar> verificacionesSostenibilidad = verificarLogica.TableroResumen(filtro);

            string rpta = JsonConvert.SerializeObject(new
            {
                verificaciones = verificaciones.Select(x => new
                {
                    x.Id,
                    x.EmpleadoId,
                    x.NumeroMes,
                    x.SemanaMes,
                    x.PuntajeMaximo,
                    x.PuntajeObtenido,
                    Verificacion = new { x.Verificacion.TipoVerificacionId, x.Verificacion.VP },
                    x.VP,
                    x.IGP
                }),
                verificacionesSostenibilidad = verificacionesSostenibilidad.Select(x => new
                {
                    x.Id,
                    x.EmpleadoId,
                    x.NumeroMes,
                    x.SemanaMes,
                    x.PuntajeMaximo,
                    x.PuntajeObtenido,
                    Verificacion = new { x.Verificacion.TipoVerificacionId, x.Verificacion.VP },
                    x.VP,
                    x.IGP
                }),
                semanas,
                comentarios,
                meses
            });

            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult GuardarTablero(VerificacionModel.GuardarTablero model)
    {
        try
        {
            Validar(model);
            if (!ModelState.IsValid) return Validation();

            verificarLogica.Guardar(model.Semanas, model.Comentarios, model.Meses);
            return Content("Ok");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult Reporte(VerificacionModel.Reporte model)
    {
        try
        {
            ExcelPackage paquete = new ExcelPackage(new System.IO.FileInfo(Path.Combine(entorno.WebRootPath, "reports", "rptVerificacion.xlsx")));
            ExcelWorksheet worksheet = paquete.Workbook.Worksheets[1];

            DateTime fechaDesde = DateTime.Parse(model.FechaDesde),
                fechaHasta = DateTime.Parse(model.FechaHasta);

            List<Verificar> lista = verificarLogica.Reporte(model.TableroId, fechaDesde, fechaHasta);

            if (lista != null)
            {
                int fila = 1;
                foreach (var item in lista)
                {
                    worksheet.Cells[fila + 1, 1].Value = fila;
                    worksheet.Cells[fila + 1, 2].Value = item.FechaRegistro.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[fila + 1, 3].Value = item.Tablero.Descripcion;
                    worksheet.Cells[fila + 1, 4].Value = item.Verificacion.TipoVerificacion.Nombre;
                    worksheet.Cells[fila + 1, 5].Value = item.Verificacion.Nombre;
                    worksheet.Cells[fila + 1, 6].Value = item.Estructura.Descripcion;
                    worksheet.Cells[fila + 1, 7].Value = string.Format("{0} {1} {2}", item.Empleado.Nombre, item.Empleado.ApellidoPaterno, item.Empleado.ApellidoMaterno);
                    worksheet.Cells[fila + 1, 8].Value = item.Encargado;
                    worksheet.Cells[fila + 1, 9].Value = item.PuntajeObtenido;
                    worksheet.Cells[fila + 1, 10].Value = item.PuntajeMaximo;
                    worksheet.Cells[fila + 1, 11].Value = (double.Parse(item.PuntajeObtenido.ToString()) / double.Parse(item.PuntajeMaximo.ToString()) * 100).ToString("##0.00") + " %";
                    worksheet.Cells[fila + 1, 12].Value = item.Fortaleza;
                    worksheet.Cells[fila + 1, 13].Value = item.Oportunidad;

                    fila++;
                }
            }

            byte[] data = paquete.GetAsByteArray();

            worksheet.Dispose();
            paquete.Dispose();

            return File(data, "application/octet-stream", string.Concat("Confirmaciones_", DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".xlsx"));
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void Validar(Verificacion model)
    {
        ModelState.Clear();
        if (model.TipoVerificacionId <= 0) ModelState.AddModelError("TipoVerificacionId", "Es necesario seleccionar un tipo de verificación a la verificación.");
        if (string.IsNullOrEmpty(model.Nombre)) ModelState.AddModelError("Nombre", "Es necesario ingresar un nombre a la verificación.");
        if (model.Categorias == null || model.Categorias.Where(x => !x.Eliminado).Count() == 0) ModelState.AddModelError("Categorias", "Es necesario agregar categorias.");
        else
        {
            int i = 1, y, z;
            foreach (var item in model.Categorias)
            {
                item.Orden = i;
                if (string.IsNullOrWhiteSpace(item.Descripcion)) ModelState.AddModelError("Categorias_Descripcion", string.Format("La categoria Nº {0} no tiene una descripción.", i));
                if (item.Preguntas == null || item.Preguntas.Where(x => !x.Eliminado).Count() == 0) ModelState.AddModelError("Categorias_Preguntas", string.Format("La categoria Nº {0} no tiene preguntas.", i));
                else
                {
                    y = 1;
                    foreach (var item2 in item.Preguntas)
                    {
                        item2.Orden = y;
                        if (string.IsNullOrEmpty(item2.Descripcion)) ModelState.AddModelError("Categorias_Preguntas_Descripcion", string.Format("Dentro de la categoria Nº {0}, la pregunta Nº {1} no tiene una descripción.", i, y));
                        if (item2.Respuestas == null || item2.Respuestas.Count == 0) ModelState.AddModelError("Categorias_Preguntas_Respuestas", string.Format("Dentro de la categoria Nº {0}, la pregunta Nº {1} no tiene respuestas.", i, y));
                        else
                        {
                            z = 1;
                            foreach (var item3 in item2.Respuestas)
                            {
                                if (string.IsNullOrEmpty(item3.Descripcion)) ModelState.AddModelError("Categorias_Preguntas_Respuesta_Descripcion", string.Format("Dentro de la categoria Nº {0}, pregunta Nº {1}, la respuesta Nº {2} no tiene una descripción.", i, y, z));
                                z++;
                            }
                        }
                        y++;
                    }
                }
                i++;
            }
        }
    }

    [NonAction]
    private void Validar(Verificar model)
    {
        int i;
        ModelState.Clear();
        if (string.IsNullOrEmpty(model.Encargado)) ModelState.AddModelError("Encargado", "Es necesario ingresar un encargado.");
        if (model.TableroId <= 0) ModelState.AddModelError("TableroId", "Es necesario seleccionar un tablero.");
        if (model.EstructuraId <= 0) ModelState.AddModelError("EstructuraId", "Es necesario seleccionar una célula.");
        if (model.Respuestas == null || model.Respuestas.Count == 0) ModelState.AddModelError("Respuestas", "No puede enviar una verificación vacía.");
        else
        {
            i = 1;
            foreach (var item in model.Respuestas)
            {
                if (item.CategoriaId <= 0) ModelState.AddModelError("Respuestas_CategoriaId", string.Format("La respuesta Nº {0} no tiene una categoria asociada.", i));
                if (item.PreguntaId <= 0) ModelState.AddModelError("Respuestas_PreguntaId", string.Format("La respuesta Nº {0} no tiene una pregunta asociada.", i));
                i++;
            }
        }
        if (model.PlanesAccion != null)
        {
            i = 1;
            foreach (var item in model.PlanesAccion)
            {
                if (string.IsNullOrWhiteSpace(item.Descripcion)) ModelState.AddModelError("Planes_Accion", string.Format("El plan de acción Nº {0} no tiene una descripción.", i));
                else if (item.Descripcion.Length > 30) ModelState.AddModelError("Planes_Accion", string.Format("La descripción del plan de acción Nº {0} sobrepasa la cantidad de 30 caracteres.", i));
                i++;
            }
        }
    }

    [NonAction]
    private void Validar(VerificacionModel.GuardarTablero model)
    {
        int i;
        ModelState.Clear();
        if (model.Semanas != null)
        {
            i = 1;
            foreach (var item in model.Semanas)
            {
                if (item.EstructuraId <= 0) ModelState.AddModelError("Semanas_Estructura_" + i, string.Format("El registro {0} no tiene un tablero asociado.", i));
                if (item.EmpleadoId <= 0) ModelState.AddModelError("Semanas_EmpleadoId_" + i, string.Format("El registro {0} no tiene un empleado asociado.", i));
                if (item.TipoVerificacionId <= 0) ModelState.AddModelError("Semanas_TipoVerificacionId_" + i, string.Format("El registro {0} no tiene un tipo de verificación asociado.", i));
                if (item.Anio <= 0) ModelState.AddModelError("Semanas_Anio_" + i, string.Format("El registro {0} no tiene un año asociado.", i));
                if (item.Mes <= 0) ModelState.AddModelError("Semanas_Mes_" + i, string.Format("El registro {0} no tiene un mes asociado.", i));
                if (item.NroSemana < 0) ModelState.AddModelError("Semanas_NroSemana_" + i, string.Format("El registro {0} tiene una cantidad de semanas negativo.", i));
                i++;
            }
        }
        if (model.Comentarios != null)
        {
            i = 1;
            foreach (var item in model.Comentarios)
            {
                if (item.EstructuraId <= 0) ModelState.AddModelError("Comentarios_Estructura_" + i, string.Format("El registro {0} no tiene un tablero asociado.", i));
                if (item.EmpleadoId <= 0) ModelState.AddModelError("Comentarios_EmpleadoId_" + i, string.Format("El registro {0} no tiene un empleado asociado.", i));
                if (item.Anio <= 0) ModelState.AddModelError("Comentarios_Anio_" + i, string.Format("El registro {0} no tiene un año asociado.", i));
                if (item.Mes <= 0) ModelState.AddModelError("Comentarios_Mes_" + i, string.Format("El registro {0} no tiene un mes asociado.", i));
                i++;
            }
        }
    }

    #endregion
}
