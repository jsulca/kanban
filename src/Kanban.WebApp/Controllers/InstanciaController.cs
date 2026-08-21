using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class InstanciaController(IInstanciaLogica instanciaLogica, IColorLogica colorLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetInstancia")
    {
        try
        {
            ViewBag.Colores = colorLogica.Listar() ?? new List<Color>();

            ViewBag.CallBack = callBack;

            Instancia model = new Instancia { };
            return PartialView("_Nuevo", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Nuevo(Instancia model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                instanciaLogica.Guardar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetInstancia")
    {
        try
        {
            ViewBag.Colores = colorLogica.Listar() ?? new List<Color>();

            ViewBag.CallBack = callBack;

            Instancia model = instanciaLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Instancia model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                instanciaLogica.Actualizar(model);
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
            List<Instancia> lista = instanciaLogica.Listar() ?? new List<Instancia>();
            string respuesta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.Abreviatura,
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
    private void Validar(Instancia model)
    {
        ModelState.Clear();
        if (model.ColorFondoId <= 0) ModelState.AddModelError("ColorFondoId", "Seleccione un color de fondo.");
        if (model.ColorTextoId <= 0) ModelState.AddModelError("ColorTextoId", "Seleccione un color de texto.");
        if (string.IsNullOrWhiteSpace(model.Descripcion)) ModelState.AddModelError("Descripcion", "Ingrese una descripcion");
    }

    #endregion
}
