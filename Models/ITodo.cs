namespace todo_universe.Models
{
    public interface ITodo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsComplete { get; set; } 
    }
}
