namespace todo_universe.Services;
public interface ITodoQueryParams
{
    public string? title { get; set; }
    public int? id { get; set; }
    public bool? isComplete { get; set; }
    public int? categoryId { get; set; }
    public int? orderByTitle { get; set; }
    public int? orderByCreatedAt { get; set; }
    public int? orderByUpdatedAt { get; set; }
    public int? orderByRemindAt { get; set; }
}
