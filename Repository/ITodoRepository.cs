using todo_universe.Models;

namespace todo_universe.Repository;
public interface ITodoRepository
{
    public Task<Todo> AddTodoAsync(Todo todo);
    public Task<Todo> DeleteTodoAsync(int id);
    public Task<Todo> EditTodoAsync(int id, Todo editedTodo);
    public Task<Todo> GetTodoByIdAsync(int id);
    public Task<List<Todo>> GetTodosAsync();
}
