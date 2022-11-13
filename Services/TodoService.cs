using Microsoft.EntityFrameworkCore;
using todo_universe.Data;
using todo_universe.Models;

namespace todo_universe.Services;

public class TodoService
{
    private readonly AppDbContext _dbContext;
    public TodoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Get all todos
    /// </summary>
    /// //System.InvalidOperationException: Could not create an instance of type 'todo_universe.Services.ITodoQueryParams'. Model bound complex types must not be abstract or value types and must have a parameterless constructor. Record types must have a single primary constructor. Alternatively, give the 'queryParams' parameter a non-null default value.
    public async Task<IEnumerable<Todo>> GetAllTodosAsync(string? title, int? id, bool? isComplete, int? categoryId, int? orderByTitle, int? orderByCreatedAt, int? orderByUpdatedAt, int? orderByRemindAt)
    {
        var todos = _dbContext.Todos.AsQueryable();
        if (title != null)
        {
            todos = todos.Where(t => t.Title.Contains(title));
        }
        if (id != null)
        {
            todos = todos.Where(t => t.Id == id);
        }
        if (isComplete != null)
        {
            todos = todos.Where(t => t.IsComplete == isComplete);
        }
        if (categoryId != null)
        {
            todos = todos.Where(t => t.CategoryId == categoryId);
        }
        if (orderByTitle != null)
        {
            todos = orderByTitle == 1 ? todos.OrderBy(t => t.Title) : todos.OrderByDescending(t => t.Title);
        }
        if (orderByCreatedAt != null)
        {
            todos = orderByCreatedAt == 1 ? todos.OrderBy(t => t.CreatedAt) : todos.OrderByDescending(t => t.CreatedAt);
        }
        if (orderByUpdatedAt != null)
        {
            todos = orderByUpdatedAt == 1 ? todos.OrderBy(t => t.UpdatedAt) : todos.OrderByDescending(t => t.UpdatedAt);
        }
        if (orderByRemindAt != null)
        {
            todos = orderByRemindAt == 1 ? todos.OrderBy(t => t.RemindAt) : todos.OrderByDescending(t => t.RemindAt);
        }
        return await todos.ToListAsync();
    }
    //{
    //    var todos = _dbContext.Todos.AsQueryable();
    //    if (!String.IsNullOrEmpty(queryParam.title))
    //    {
    //        todos = todos.Where(todo => todo.Title.Contains(queryParam.title));
    //    }

    //    if (queryParam.id != null || queryParam.id is not null)
    //    {
    //        todos = todos.Where(todo => todo.Id == queryParam.id);
    //    }

    //    if (queryParam.isComplete != null)
    //    {
    //        todos = todos.Where(todo => todo.IsComplete == queryParam.isComplete);
    //    }


    //    if (queryParam.categoryId != null)
    //    {
    //        todos = todos.Where(todo => todo.CategoryId == queryParam.categoryId);
    //    }

    //    if (queryParam.orderByTitle == 1)
    //    {
    //        todos = todos.OrderBy(todo => todo.Title);
    //    }
    //    else if (queryParam.orderByTitle == 2)
    //    {
    //        todos = todos.OrderByDescending(todo => todo.Title);
    //    }

    //    if (queryParam.orderByCreatedAt == 1)
    //    {
    //        todos = todos.OrderBy(todo => todo.CreatedAt);
    //    }
    //    else if (queryParam.orderByCreatedAt == 2)
    //    {
    //        todos = todos.OrderByDescending(todo => todo.CreatedAt);
    //    }

    //    if (queryParam.orderByUpdatedAt == 1)
    //    {
    //        todos = todos.OrderBy(todo => todo.UpdatedAt);
    //    }
    //    else if (queryParam.orderByUpdatedAt == 2)
    //    {
    //        todos = todos.OrderByDescending(todo => todo.UpdatedAt);
    //    }

    //    if (queryParam.orderByRemindAt == 1)
    //    {
    //        todos = todos.OrderBy(todo => todo.RemindAt);
    //    }
    //    else if (queryParam.orderByRemindAt == 2)
    //    {
    //        todos = todos.OrderByDescending(todo => todo.RemindAt);
    //    }

    //    return await todos.ToListAsync();
    //}
}
