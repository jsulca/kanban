using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Net;
using System.Security.Claims;

namespace Kanban.WebApp.Api;

[ApiController]
[Route("api/compromiso")]
[Authorize]
public class CompromisoApiController(
    IEstructuraLogica estructuraLogica,
    ICompromisoLogica compromisoLogica,
    IOrigenLogica origenLogica,
    IIndicadorLogica indicadorLogica,
    IUsuarioLogica usuarioLogica) : ControllerBase
{
    private readonly List<string> _errors = new List<string>();

    #region Acciones

    [Route("listar")]
    [HttpPost]
    public IActionResult Listar(CompromisoModel.Listar model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            //TODO: OBTENER LA ESTRUCTURA DEL USUARIO
            int usuarioId = GetUserID();
            List<UsuarioEstructura> estructuras = usuarioLogica.BuscarPorUsuario(usuarioId);
            int[] estructurasId = estructuras?.Select(x => x.EstructuraId).ToArray();

            var paginado = compromisoLogica.ListarPorPagina(new CompromisoFiltro { Estructuras = estructurasId }, model.PageNumber, model.PageSize);
            var compromisos = paginado.Items;
            var totalRows = paginado.TotalRows;
            respuesta.response.codigo = "0000";
            respuesta.response.descripcion = "Ok";

            double d = double.Parse(totalRows.ToString()) / double.Parse(model.PageSize.ToString());
            int cantidadPaginas = (int)Math.Ceiling(d);
            RGBA colorFondo, colorTexto;

            respuesta.data = new
            {
                lista = compromisos.Select(x =>
                {
                    colorFondo = new RGBA(x.Area?.ColorFondo?.Rgba ?? "(248, 249, 250, 1)");
                    colorTexto = new RGBA(x.Area?.ColorTexto?.Rgba ?? "(33, 37, 41, 1)");

                    return new
                    {
                        id = x.Id,
                        codigo = x.Codigo,
                        descripcion = x.Descripcion,
                        fecha = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                        estado = x.Estado.ToString().Replace("_", " "),
                        tablero = new { x.Tablero.Descripcion },
                        colorFondo = new
                        {
                            red = colorFondo.Red,
                            green = colorFondo.Green,
                            blue = colorFondo.Blue,
                            alpha = colorFondo.Alpha
                        },
                        colorTexto = new
                        {
                            red = colorTexto.Red,
                            green = colorTexto.Green,
                            blue = colorTexto.Blue,
                            alpha = colorTexto.Alpha
                        },
                        areaId = x.AreaId,
                        area = new { descripcion = x.Area?.Descripcion },
                        instanciaId = x.InstanciaId,
                        instancia = new { abreviatura = x.Instancia?.Abreviatura },
                        fechaProgramacion = x.FechaProgramacion?.ToString("dd/MM/yyyy"),
                        fechaReprogramacion = x.FechaReprogramacion?.ToString("dd/MM/yyyy"),
                        estructura = new { descripcion = x.Estructura.Descripcion }
                    };
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

    [Route("buscar")]
    [HttpPost]
    public IActionResult Buscar(CompromisoModel.Buscar model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            if (!model.Id.HasValue)
            {
                respuesta.response.codigo = "0001";
                respuesta.response.descripcion = "Error de validación de datos.";
                respuesta.response.comentario = "Es necesario ingresar un identificador.";
            }
            else
            {
                Compromiso compromiso = compromisoLogica.Buscar(model.Id.Value, true);

                #region Llenar el seguimiento

                var fechas = compromiso.Estados.Select(x => x.FechaRegistro.Date).ToList();
                if (compromiso.Instancias != null)
                {
                    fechas.AddRange(compromiso.Instancias.Select(x => x.FechaRegistro.Date).ToList());
                }
                fechas = fechas.Distinct().ToList();
                List<DateTime> registros;
                CompromisoEstado compromisoEstado;
                CompromisoInstancia compromisoInstancia;

                List<Seguimiento> seguimientos = new List<Seguimiento>();
                if (fechas != null)
                {
                    Seguimiento seguimiento;
                    List<SeguimientoDetalle> detalles;
                    foreach (var item in fechas.OrderByDescending(x => x))
                    {
                        seguimiento = new Seguimiento { Fecha = item.ToString("dd MMM yyyy") };

                        registros = compromiso.Estados.Where(x => x.FechaRegistro.Date == item).Select(x => x.FechaRegistro).ToList();
                        if (compromiso.Instancias != null)
                        {
                            registros.AddRange(compromiso.Instancias.Where(x => x.FechaRegistro.Date == item).Select(x => x.FechaRegistro).ToList());
                        }
                        detalles = new List<SeguimientoDetalle>();

                        foreach (var item2 in registros.OrderByDescending(x => x))
                        {

                            compromisoEstado = compromiso.Estados.Where(x => x.FechaRegistro == item2).SingleOrDefault();

                            if (compromisoEstado != null)
                            {
                                detalles.Add(new SeguimientoDetalle
                                {
                                    Hora = compromisoEstado.FechaRegistro.ToString("HH:mm"),
                                    Header = compromisoEstado.Estado.ToString().Replace("_", " "),
                                    Body = compromisoEstado.Motivo,
                                    Footer = GetBody(compromisoEstado.Usuario, compromisoEstado.Empleado)
                                });
                            }
                            if (compromiso.Instancias != null)
                            {
                                compromisoInstancia = compromiso.Instancias.Where(x => x.FechaRegistro == item2).SingleOrDefault();
                                if (compromisoInstancia != null)
                                {
                                    detalles.Add(new SeguimientoDetalle
                                    {
                                        Hora = compromisoInstancia.FechaRegistro.ToString("HH:mm"),
                                        Header = compromisoInstancia.Instancia.Descripcion,
                                        Body = compromisoInstancia.Motivo,
                                        Footer = GetBody(compromisoInstancia.Usuario, compromisoInstancia.Empleado)
                                    });
                                }
                            }
                        }
                        seguimiento.Detalles = detalles;
                        seguimientos.Add(seguimiento);
                    }
                }

                #endregion

                respuesta.response.codigo = "0000";
                respuesta.response.descripcion = "Ok";
                respuesta.data = new
                {
                    codigo = compromiso.Codigo,
                    reportado = string.Format("{0} {1} {2}", compromiso.EmpleadoRegistro.Nombre, compromiso.EmpleadoRegistro.ApellidoPaterno, compromiso.EmpleadoRegistro.ApellidoMaterno),
                    tablero = compromiso.Tablero.Descripcion,
                    estructura = compromiso.Estructura.Descripcion,
                    fechaRegistro = compromiso.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    origen = compromiso.Origen,
                    indicador = compromiso.Impacto,
                    foto = compromiso.FotoId.HasValue ? "/Adjunto/Index/" + compromiso.FotoId : null,
                    seguimiento = seguimientos?.Select(x => new
                    {
                        fecha = x.Fecha,
                        detalles = x.Detalles?.Select(y => new
                        {
                            hora = y.Hora,
                            header = y.Header,
                            body = y.Body,
                            footer = y.Footer
                        })
                    })
                };
            }
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
    public IActionResult Guardar()
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            CompromisoModel.Post model = new CompromisoModel.Post
            {
                UsuarioId = LeerEntero("UsuarioId"),
                EmpleadoId = LeerEntero("EmpleadoId"),
                EstructuraId = LeerEntero("EstructuraId"),
                TableroId = LeerEntero("TableroId"),
                Descripcion = LeerTexto("Descripcion"),
                Detalle = LeerTexto("Detalle"),
                Origen = LeerTexto("Origen"),
                Impacto = LeerTexto("Indicador"),
                Foto = Request.HasFormContentType && Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null
            };

            Validar(model);
            if (_errors.Count == 0)
            {
                Compromiso entidad = model.Get();
                entidad.Estado = EstadoCompromiso.NUEVO;

                if (model.Foto != null) entidad.Foto = AlmacenArchivos.Guardar(model.Foto);

                compromisoLogica.Guardar(entidad, model.UsuarioId, model.EmpleadoId);

                respuesta.response.codigo = "0000";
                respuesta.response.descripcion = "Ok";
                respuesta.data = new { entidad.Id };
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

    [Route("parametros")]
    [HttpPost]
    public IActionResult Parametros()
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {

            List<Origen> origenes = origenLogica.Listar(new OrigenFiltro { Activo = true });
            List<Indicador> indicadores = indicadorLogica.Listar(new IndicadorFiltro { Activo = true });

            respuesta.response.codigo = "0000";
            respuesta.response.descripcion = "Ok";
            respuesta.data = new
            {
                origenes = origenes?.Select(x => new { nombre = x.Nombre }),
                impactos = indicadores?.Select(x => new { nombre = x.Nombre })
            };
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    [Route("asignados")]
    [HttpPost]
    public IActionResult Asignados(CompromisoModel.Asignados model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            CompromisoFiltro filtro = new CompromisoFiltro();
            filtro.ResponsableId = model.EmpleadoId;
            filtro.InstanciaId = 0;
            filtro.Estados = new int[] { (int)EstadoCompromiso.PROGRAMADO, (int)EstadoCompromiso.REPROGRAMADO };

            var paginado = compromisoLogica.ListarPorPagina(filtro, model.PageNumber, model.PageSize);
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
                    codigo = x.Codigo,
                    descripcion = x.Descripcion,
                    fecha = x.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    estado = x.Estado.ToString().Replace("_", " "),
                    tablero = new { x.Tablero.Descripcion },
                    colorFondo = new
                    {
                        clase = x.Area?.ColorFondo.Clase,
                        hex = x.Area?.ColorFondo.Hex,
                        rgba = x.Area?.ColorFondo.Rgba,
                    },
                    colorTexto = new
                    {
                        clase = x.Area?.ColorTexto.Clase,
                        hex = x.Area?.ColorTexto.Hex,
                        rgba = x.Area?.ColorTexto.Rgba,
                    },
                    areaId = x.AreaId,
                    area = new { descripcion = x.Area?.Descripcion },
                    instanciaId = x.InstanciaId,
                    instancia = new { abreviatura = x.Instancia?.Abreviatura },
                    fechaProgramacion = x.FechaProgramacion?.ToString("dd/MM/yyyy"),
                    fechaReprogramacion = x.FechaReprogramacion?.ToString("dd/MM/yyyy"),
                    estructura = new { descripcion = x.Estructura.Descripcion }
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

    [Route("verificar")]
    [HttpPost]
    public IActionResult Verificar(CompromisoModel.VerificarAPI model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            Compromiso compromiso = compromisoLogica.Buscar(model.Id);
            compromiso.Estado = EstadoCompromiso.POR_VERIFICAR;
            compromisoLogica.CambiarEstado(compromiso, null, model.UsuarioId, model.EmpleadoId);

            respuesta.response.codigo = "0000";
            respuesta.response.descripcion = "Ok";
            respuesta.data = new
            {
                mensaje = "El compromiso paso al estado POR VERIFICAR"
            };
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    [Route("resumen")]
    [HttpPost]
    public IActionResult Resumen(CompromisoModel.Resumen model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            Validar(model);
            if (_errors.Count == 0)
            {
                int usuarioId = GetUserID();
                List<UsuarioEstructura> estructuras = usuarioLogica.BuscarPorUsuario(usuarioId);
                int[] estructurasId = estructuras?.Select(x => x.EstructuraId).ToArray();

                List<TableroResumen> tableros = estructuraLogica.Listar(estructurasId);
                respuesta.response.codigo = "0000";
                respuesta.response.descripcion = "Ok";
                respuesta.data = tableros.Select(x => new
                {
                    estructuraId = x.EstructuraId,
                    nombre = x.Nombre,
                    pendiente = x.Pendiente,
                    nuevo = x.Nuevo,
                    porVerificar = x.PorVerificar,
                    fueraFecha = x.FueraFecha
                });
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
    private void Validar(CompromisoModel.Post model)
    {
        if (!model.UsuarioId.HasValue || model.UsuarioId.Value <= 0) _errors.Add("Es necesario asociar un usuario al compromiso.");
        if (!model.EmpleadoId.HasValue || model.EmpleadoId.Value <= 0) _errors.Add("Es necesario asociar un empleado al compromiso.");
        if (!model.EstructuraId.HasValue || model.EstructuraId.Value <= 0) _errors.Add("Es necesario asociar una estructura al compromiso.");
        if (!model.TableroId.HasValue || model.TableroId.Value <= 0) _errors.Add("Es necesario seleccionar el tablero.");
        if (string.IsNullOrWhiteSpace(model.Descripcion)) _errors.Add("Es necesario ingresar una breve descripción.");
        else if (model.Descripcion.Length > 30) _errors.Add("La breve descripción solo puede contar con 30 caracteres.");
    }

    [NonAction]
    private string GetBody(Usuario usuario, Empleado empleado)
    {
        string rpta = "";
        if (usuario != null) rpta += string.Format("Usuario: {0}", usuario.Nombre);
        if (empleado != null)
        {
            if (usuario != null) rpta += "\n";
            rpta += string.Format("Empleado: {0} {1}", empleado.Nombre, empleado.ApellidoPaterno);
        }
        return rpta;
    }

    [NonAction]
    private void Validar(CompromisoModel.Resumen model)
    {
        if (model == null) _errors.Add("No existen tableros a consultar.");
    }

    /// <summary>
    ///     Sustituye a <c>Request.Params</c>, que en .NET Framework consultaba a la
    ///     vez el formulario y la query string.
    /// </summary>
    [NonAction]
    private string? LeerTexto(string clave)
    {
        if (Request.HasFormContentType && Request.Form.TryGetValue(clave, out var delFormulario))
            return delFormulario;

        if (Request.Query.TryGetValue(clave, out var deLaQuery)) return deLaQuery;

        return null;
    }

    [NonAction]
    private int? LeerEntero(string clave)
    {
        var valor = LeerTexto(clave);
        return valor != null ? int.Parse(valor) : null;
    }

    [NonAction]
    private int GetUserID()
    {
        var identity = User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            IEnumerable<Claim> claims = identity.Claims;
            string userId = claims.SingleOrDefault(x => x.Type == ClaimTypes.PrimarySid)?.Value ?? "0";
            return int.Parse(userId);
        }
        else return 0;
    }

    #endregion

    #region Clases

    private class Seguimiento
    {
        public string Fecha { get; set; }
        public List<SeguimientoDetalle> Detalles { get; set; }
    }

    private class SeguimientoDetalle
    {
        public string Header { get; set; }
        public string Hora { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
    }

    private class RGBA
    {
        public RGBA(string color)
        {
            color = color.Replace("(", "");
            color = color.Replace(")", "");
            color = color.Replace(" ", "");

            string[] colores = color.Split(',');
            Red = int.Parse(colores[0]);
            Green = int.Parse(colores[1]);
            Blue = int.Parse(colores[2]);
            Alpha = decimal.Parse(colores[3]);
        }

        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public decimal Alpha { get; set; }

    }

    #endregion
}
