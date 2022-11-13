using Microsoft.EntityFrameworkCore;
using todo_universe.Data;
using todo_universe.Models;

namespace todo_universe.Repository;
public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _dbContext;

    public TodoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Todo> AddTodoAsync(Todo todo)
    {
        await _dbContext.Todos.AddAsync(todo);
        await _dbContext.SaveChangesAsync();
        return todo;
    }

    public async Task<Todo> DeleteTodoAsync(int id)
    {
        var todo = await _dbContext.Todos.FindAsync(id);
        
        if (todo == null)
        {
            return null;
        }

        _dbContext.Todos.Remove(todo);
        await _dbContext.SaveChangesAsync();
        return todo;
    }

    public async Task<Todo> EditTodoAsync(int id,Todo editedTodo)
    {
        var todo = await _dbContext.Todos.FindAsync(id);

        if (todo == null) return null;

        todo.Title = editedTodo.Title;
        todo.IsComplete = editedTodo.IsComplete;
        todo.CategoryId = editedTodo.CategoryId;
        todo.RemindAt = editedTodo.RemindAt;
        todo.UpdatedAt = DateTime.Now;

        _dbContext.Entry(todo).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        return editedTodo;
    }

    //TODO : User can get any todo by id even if not user .. need to fix
    public async Task<Todo> GetTodoByIdAsync(int id)
    {
        return await _dbContext.Todos.FindAsync(id);
    }

    public async Task<List<Todo>> GetTodosAsync()
    {
        return await _dbContext.Todos.ToListAsync();
    }
}
