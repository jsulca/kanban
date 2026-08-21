using Newtonsoft.Json;
using System.Net;

namespace Kanban.WebApp.Areas.Seguridad.Controllers;

[SafetyFilter(NoValidarAccion = true)]
[Area("Seguridad")]
public class SolicitudController(ISolicitudLogica solicitudLogica) : Controller
{

    #region Acciones

    public ActionResult Index()
    {
        return View();
    }

    public ActionResult ListarPorPagina(SolicitudFiltro filter, int pageIndex, int pageSize)
    {
        try
        {
            var paginado = solicitudLogica.ListarPorPagina(filter, pageIndex, pageSize);
            var lista = paginado.Items;
            var totalRows = paginado.TotalRows;
            string rpta = JsonConvert.SerializeObject(new
            {
                lista = lista.Select(x => new
                {
                    x.Id,
                    x.Nombre,
                    x.Apellido,
                    x.NroDocumento,
                    x.Correo,
                    x.Telefono,
                    FechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm")
                }),
                totalRows
            });
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.Message = ex.Message;
            return PartialView("_Error");
        }
    }

    #endregion
}
