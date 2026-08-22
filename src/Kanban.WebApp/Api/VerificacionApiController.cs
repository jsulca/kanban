
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Verificaciones;
using Kanban.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Api;

[ApiController]
[Route("api/confirmacion")]
[Authorize]
public class VerificacionApiController(
    IVerificarLogica verificarLogica,
    IVerificacionLogica verificacionLogica,
    IEstructuraLogica estructuraLogica,
    IEstructuraEmpleadoLogica estructuraEmpleadoLogica,
    ISostenibilidadLogica sostenibilidadLogica) : ControllerBase
{
    private readonly List<string> _errors = new List<string>();

    #region Acciones

    [Route("listar")]
    [HttpPost]
    public IActionResult Listar(VerificacionModel.Listar model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {

            model.PageSize = 20;
            var paginado = verificarLogica.ListarPorPagina(new VerificarFiltro { EmpleadoId = model.EmpleadoId }, model.PageNumber, model.PageSize);
            var compromisos = paginado.Items;
            var totalRows = paginado.TotalRows;
            respuesta.response.codigo = "0000";
            respuesta.response.descripcion = "Ok";

            double d = double.Parse(totalRows.ToString()) / double.Parse(model.PageSize.ToString());
            int cantidadPaginas = (int)Math.Ceiling(d);

            respuesta.data = new
            {
                lista = compromisos.Select(x => new
                {
                    id = x.Id,
                    encargado = x.Encargado,
                    fechaRegistro = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    puntajeMaximo = x.PuntajeMaximo,
                    puntajeObtenido = x.PuntajeObtenido,
                    verificacion = new { nombre = x.Verificacion.Nombre },
                }).ToList(),
                totalRows,
                pages = cantidadPaginas
            };
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    [Route("listartipoconfirmacion")]
    [HttpPost]
    public IActionResult ListarTipoConfirmacion(VerificacionModel.ListarTipoVerificacion model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {

            model.PageSize = 20;
            var paginado = verificacionLogica.ListarPorPagina(new VerificacionFiltro { Activo = true }, model.PageNumber, model.PageSize);
            var compromisos = paginado.Items;
            var totalRows = paginado.TotalRows;
            respuesta.response.codigo = "0000";
            respuesta.response.descripcion = "Ok";

            double d = double.Parse(totalRows.ToString()) / double.Parse(model.PageSize.ToString());
            int cantidadPaginas = (int)Math.Ceiling(d);

            respuesta.data = new
            {
                lista = compromisos.Select(x => new
                {
                    id = x.Id,
                    nombre = x.Nombre,
                    activo = x.Activo,
                    rom = x.Rom,
                    tipoVerificacionId = x.TipoVerificacionId,
                    tipoVerificacion = new { nombre = x.TipoVerificacion.Nombre }
                }).ToList(),
                totalRows,
                pages = cantidadPaginas
            };
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    [Route("listarcelula")]
    [HttpPost]
    public IActionResult ListarCelula(VerificacionModel.ListarCelula model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            List<Estructura> lista = estructuraLogica.Arbol(model.Id);
            respuesta.response.codigo = "0000";
            respuesta.response.descripcion = "Ok";
            respuesta.data = lista.Select(x => new
            {
                x.Id,
                x.Descripcion
            });
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    [Route("buscartipoconfirmacion")]
    [HttpPost]
    public IActionResult BuscarTipoConfirmacion(VerificacionModel.BuscarTipoVerificacion model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            Verificacion verificacion = verificacionLogica.Buscar(model.Id, true);

            respuesta.response.codigo = "0000";
            respuesta.response.descripcion = "Ok";
            respuesta.data = new
            {
                tipoConfirmacionId = verificacion.Id,
                nombre = verificacion.Nombre,
                instruccion = verificacion.Instruccion,
                fortaleza = verificacion.Fortaleza,
                oportunidad = verificacion.Oportunidad,
                planAccion = verificacion.PlanAccion,
                rom = verificacion.Rom,
                instructivoEstandar = verificacion.InstructivoEstandar,
                resumenCategoria = verificacion.ResumenCategoria,
                categorias = verificacion.Categorias.Select(x => new
                {
                    categoriaId = x.Id,
                    orden = x.Orden,
                    descripcion = x.Descripcion,
                    preguntas = x.Preguntas.Select(y => new
                    {
                        preguntaId = y.Id,
                        titulo = y.Titulo,
                        descripcion = y.Descripcion,
                        orden = y.Orden,
                        respuestas = y.Respuestas.Select(z => new
                        {
                            valor = z.Valor,
                            descripcion = z.Descripcion
                        })
                    })
                })
            };
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    [Route("guardar")]
    [HttpPost]
    public IActionResult Guardar(VerificacionModel.Guardar model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {

            Validar(model);
            if (_errors.Count == 0)
            {

                List<EstructuraEmpleado> confirmadores = estructuraEmpleadoLogica.Listar(model.TableroId.Value);
                List<Sostenibilidad> confirmadoresSostenibilidad = sostenibilidadLogica.Listar(model.TableroId.Value);

                if (!confirmadores.Any(x => x.EmpleadoId == model.EmpleadoId.Value) && !confirmadoresSostenibilidad.Any(x => x.EmpleadoId == model.EmpleadoId.Value))
                {
                    respuesta.response.codigo = "0002";
                    respuesta.response.descripcion = "Error de validacion de informacion";
                    respuesta.response.comentario = "Usted no se encuentra registrado como confirmador del tablero seleccionado";
                }
                else
                {
                    Verificacion verificacion = verificacionLogica.Buscar(model.VerificacionId.Value, true);
                    Verificar verificar = model.Get();
                    verificar.VP = verificacion.VP;

                    EstructuraEmpleado confirmador = confirmadores.SingleOrDefault(x => x.EmpleadoId == model.EmpleadoId);
                    //Agregar el area asociada
                    verificar.AreaId = confirmador?.AreaId;

                    #region Llenar información faltante

                    List<Respuesta> respuestas = verificacion.Categorias[0].Preguntas[0].Respuestas;
                    int valorMaximo = respuestas.Select(x => x.Valor).Max();
                    int cantidadPreguntas = 0;
                    //Llenar la información
                    foreach (var item in verificar.Respuestas)
                    {
                        foreach (var item2 in verificacion.Categorias)
                        {
                            foreach (var item3 in item2.Preguntas)
                            {
                                foreach (var item4 in item3.Respuestas)
                                {
                                    if (item.PreguntaId == item4.PreguntaId && item.Valor == item4.Valor)
                                    {
                                        item.CategoriaId = item2.Id;
                                        item.Descripcion = item4.Descripcion;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var item2 in verificacion.Categorias) cantidadPreguntas += item2.Preguntas.Count;

                    #endregion

                    verificar.PuntajeObtenido = verificar.Respuestas.Sum(x => x.Valor);
                    verificar.PuntajeMaximo = valorMaximo * cantidadPreguntas;

                    verificarLogica.GuardarEF(verificar);
                    //verificarLogica.Guardar(verificar);

                    respuesta.response.codigo = "0000";
                    respuesta.response.descripcion = "Ok";
                    respuesta.data = new { id = verificar.Id };
                }
            }
            else
            {
                respuesta.response.codigo = "0001";
                respuesta.response.descripcion = "Error de validación de datos.";
                respuesta.response.comentario = string.Join(",", _errors);
            }
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    [Route("buscar")]
    [HttpPost]
    public IActionResult Buscar(VerificacionModel.Buscar model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            Validar(model);
            if (_errors.Count == 0)
            {
                Verificar entidad = verificarLogica.Buscar(model.Id.Value, true);

                if (entidad == null)
                {
                    respuesta.response.codigo = "0002";
                    respuesta.response.descripcion = "Error de validacion de informacion";
                    respuesta.response.comentario = "No es ha encontrado información";
                }
                else
                {
                    List<Categoria> categorias = entidad.Respuestas.GroupBy(x => x.CategoriaId).Select(x => x.First().Categoria).ToList();

                    respuesta.response.codigo = "0000";
                    respuesta.response.descripcion = "Ok";
                    respuesta.data = new
                    {
                        id = entidad.Id,
                        fechaRegistro = entidad.FechaRegistro.ToString("dd/MM/yyyy"),
                        horaRegistro = entidad.FechaRegistro.ToString("HH:mm"),
                        empleado = string.Format("{0} {1}", entidad.Empleado.Nombre, entidad.Empleado.ApellidoPaterno),
                        tablero = entidad.Tablero.Descripcion,
                        encargado = entidad.Encargado,
                        instructivoEstandar = entidad.InstructivoEstandar,
                        puntajeObtenido = entidad.PuntajeObtenido,
                        puntajeMaximo = entidad.PuntajeMaximo,
                        rom = entidad.Rom,
                        nroRom = entidad.NroRom,
                        fortaleza = entidad.Fortaleza,
                        oportunidad = entidad.Oportunidad,
                        categorias = categorias.Select(x => new
                        {
                            orden = x.Orden,
                            descripcion = x.Descripcion,
                            respuestas = entidad.Respuestas.Where(y => y.CategoriaId == x.Id).Select(y => new
                            {
                                titulo = y.Pregunta.Titulo,
                                descripcion = y.Pregunta.Descripcion,
                                respuesta = y.Descripcion,
                                valor = y.Valor
                            })
                        }),
                        planesAccion = entidad.PlanesAccion?.Select(x => new
                        {
                            descripcion = x.Descripcion
                        })
                    };
                }
            }
            else
            {
                respuesta.response.codigo = "0001";
                respuesta.response.descripcion = "Error de validación de datos.";
                respuesta.response.comentario = string.Join(",", _errors);
            }

        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }

        return Ok(respuesta);
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    public void Validar(VerificacionModel.Guardar model)
    {
        int i;
        if (!model.EmpleadoId.HasValue) _errors.Add("Falto ingresar el identificador del empleado");
        if (!model.UsuarioId.HasValue) _errors.Add("Falto ingresar el identificador del usuario");
        if (!model.TableroId.HasValue) _errors.Add("Falto ingresar el identificador del tablero");
        if (!model.VerificacionId.HasValue) _errors.Add("Falto ingresar el identificador de la verificación");
        if (!model.EstructuraId.HasValue) _errors.Add("Falto ingresar el identificador de la célula");
        if (string.IsNullOrEmpty(model.Encargado)) _errors.Add("Falto ingresar el encargado");
        if (model.Respuestas == null || model.Respuestas.Count == 0) _errors.Add("No se ha ingresado ninguna respuesta");
        else
        {
            i = 1;
            foreach (var item in model.Respuestas)
            {
                if (item.PreguntaId <= 0) _errors.Add(string.Format("La respuesta nro. {0} no tiene una pregunta asociada", i));
                if (item.Valor < 0) _errors.Add(string.Format("La respuesta nro. {0} no tiene un valor válido", i));
                i++;
            }
        }
        if (model.PlanesAccion != null)
        {
            i = 1;
            foreach (var item in model.PlanesAccion)
            {
                if (string.IsNullOrEmpty(item.Descripcion)) _errors.Add(string.Format("El plan de acción nro. {0} no tiene una descripción", i));
                else if (item.Descripcion.Length > 30) _errors.Add(string.Format("La descripción del plan de acción Nº {0} sobrepasa la cantidad de 30 caracteres.", i));
                i++;
            }
        }
    }

    [NonAction]
    public void Validar(VerificacionModel.Buscar model)
    {
        if (!model.Id.HasValue) _errors.Add("No se ingreso un identificador.");
    }

    #endregion

}
