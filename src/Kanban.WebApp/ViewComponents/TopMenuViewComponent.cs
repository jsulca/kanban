using Kanban.Application.Abstractions.UseCases.Compromiso;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.ViewComponents;

/// <summary>
///     Sustituye a <c>Html.RenderAction("TopMenu", "Home")</c> del layout, que no
///     existe en ASP.NET Core. Reutiliza la misma vista parcial que sigue devolviendo
///     <c>HomeController.TopMenu</c>.
/// </summary>
public class TopMenuViewComponent(IAlertaLogica alertaLogica) : ViewComponent
{
    private const int PageSize = 20;

    public IViewComponentResult Invoke()
    {
        var user = HttpContext.GetUser()!;

        ViewBag.Lista = alertaLogica.Listar(1, PageSize, user.EmployeeId);
        ViewBag.AlertasPendientes = alertaLogica.Pendientes(user.EmployeeId);
        ViewBag.CantidadPorPagina = PageSize;

        return View("~/Views/Shared/_TopMenu.cshtml", user);
    }
}
