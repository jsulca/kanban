namespace Kanban.WebApp.Commons;

public class TreeModel
{
    public string? id { get; set; }

    public string? text { get; set; }

    public string? icon { get; set; }

    public object data { get; set; } = new();

    public object state { get; set; } = new();

    public object li_attr { get; set; } = new();

    public object a_attr { get; set; } = new();

    public List<TreeModel> children { get; set; } = [];
}
