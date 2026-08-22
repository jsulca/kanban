using Kanban.Application.Abstractions.UseCases.Compromiso;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Controllers;

public class HomeController(IAlertaLogica alertaLogica) : Controller
{
    private const int PageSize = 20;

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult Index()
    {
        var user = HttpContext.GetUser()!;
        return View(user.Menus);
    }

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult TopMenu()
    {
        var user = HttpContext.GetUser()!;

        var lista = alertaLogica.Listar(1, PageSize, user.EmployeeId);
        var pendientes = alertaLogica.Pendientes(user.EmployeeId);

        ViewBag.Lista = lista;
        ViewBag.AlertasPendientes = pendientes;
        ViewBag.CantidadPorPagina = PageSize;

        return PartialView("_TopMenu", user);
    }

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult LeftMenu()
    {
        var user = HttpContext.GetUser()!;
        return PartialView("_LeftMenu", user);
    }

    public ActionResult Stay()
    {
        return Content("Ok");
    }

    [SafetyFilter(NoValidarAccion = true)]
    public ActionResult PageNotFound()
    {
        return View();
    }

    public ActionResult PartialError(bool isSesion = false)
    {
        if (isSesion) ViewBag.Message = "La sesión expiró.";
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return PartialView("_Error");
    }

    [SafetyFilter(NoValidarAccion = true)]
    public async Task<ActionResult> ConfirmarAlertas()
    {
        var user = HttpContext.GetUser()!;
        await alertaLogica.ConfirmarAlertasAsync(user.EmployeeId);
        return Content("Ok");
    }
}
