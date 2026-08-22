using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.ViewComponents;

/// <summary>
///     Sustituye a <c>Html.RenderAction("LeftMenu", "Home")</c> del layout.
/// </summary>
public class LeftMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var user = HttpContext.GetUser()!;
        return View("~/Views/Shared/_LeftMenu.cshtml", user);
    }
}
