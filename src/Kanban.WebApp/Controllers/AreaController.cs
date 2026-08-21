using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class AreaController(IAreaLogica areaLogica, IColorLogica colorLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index() => View();

    public ActionResult Nuevo(string callBack = "SetArea")
    {
        try
        {
            ViewBag.Colores = colorLogica.Listar() ?? new List<Color>();

            ViewBag.CallBack = callBack;

            Area model = new Area { };
            return PartialView("_Nuevo", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Nuevo(Area model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                areaLogica.Guardar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetArea")
    {
        try
        {
            ViewBag.Colores = colorLogica.Listar() ?? new List<Color>();

            ViewBag.CallBack = callBack;

            Area model = areaLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Area model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                areaLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar()
    {
        try
        {
            List<Area> lista = areaLogica.Listar() ?? new List<Area>();
            string respuesta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Descripcion
            }));
            return Content(respuesta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void Validar(Area model)
    {
        ModelState.Clear();
        if (model.ColorFondoId <= 0) ModelState.AddModelError("ColorFondoId", "Seleccione un color de fondo.");
        if (model.ColorTextoId <= 0) ModelState.AddModelError("ColorTextoId", "Seleccione un color de texto.");
        if (string.IsNullOrWhiteSpace(model.Descripcion)) ModelState.AddModelError("Descripcion", "Ingrese una descripción.");
    }

    #endregion
}
