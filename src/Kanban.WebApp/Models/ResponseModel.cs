namespace Kanban.WebApp.Models;

public class ResponseModel
{
    public Response response { get; set; } = new Response();
    public object? data { get; set; }
}

public class Response
{
    public string? codigo { get; set; }
    public string? descripcion { get; set; }
    public string? comentario { get; set; }
}
