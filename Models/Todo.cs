namespace todo_universe.Models
{
    public class Todo : ITodo
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public bool IsComplete { get; set; } = false;
    }
}
