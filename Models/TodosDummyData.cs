namespace todo_universe.Models
{
    public class TodosDummyData
    {
        public List<Todo> GenerateDummyTodos()
        {
            var todos = new List<Todo>();
            todos.Add(new Todo { Id=1,Title="dummy1" });
            todos.Add(new Todo { Id=2,Title="dummy2" });
            todos.Add(new Todo { Id=3,Title= "dummy3" });
            return todos;
        }
    }
}
