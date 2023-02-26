using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Authentication;
using todo_universe.Data;
using todo_universe.Models;
using todo_universe.Manager;
using todo_universe.Repository;
using todo_universe.ActionFilters;
using System.Security.Claims;
using todo_universe.Services;

namespace todo_universe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController : ApiController
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TodosController> _logger;
    private readonly JwtAuthenticationManager _jwtAuthManager;
    private readonly ITodoRepository _todoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly TodoService _todoService;
    
    public TodosController(ILogger<TodosController> logger, AppDbContext dbContext, JwtAuthenticationManager jwtAuthenticationManager, ITodoRepository todoRepository, ICategoryRepository categoryRepository,TodoService todoService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _jwtAuthManager = jwtAuthenticationManager;
        _todoRepository = todoRepository;
        _categoryRepository = categoryRepository;
        _todoService = todoService;
    }

    [Authorize]
    [HttpGet]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Index([FromQuery] string? title, [FromQuery] int? id, [FromQuery] bool? isComplete,[FromQuery] int? categoryId,[FromQuery] int? orderByTitle,[FromQuery] int? orderByCreatedAt,[FromQuery] int? orderByUpdatedAt,[FromQuery] int? orderByRemindAt)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _logger.LogInformation("##############=> getting all todos");

        List<Todo> todos = _todoService.GetAllTodosAsync(int.Parse(UserId), title, id, isComplete, categoryId, orderByTitle, orderByCreatedAt, orderByUpdatedAt, orderByRemindAt).Result.ToList();

        var categories = _categoryRepository.GetCategoriesAsync(int.Parse(UserId)).Result.ToList();

        var todosWithCategory = todos.Select(todo => new
        {
            todo.Id,
            todo.Title,
            todo.IsComplete,
            todo.CreatedAt,
            todo.UpdatedAt,
            todo.RemindAt,
            todo.CategoryId,
            Category = categories.FirstOrDefault(c => c.Id == todo.CategoryId)
        });

        return Ok(todosWithCategory);
    }

    [Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Add(Todo todo)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        todo.UserId = int.Parse(userId);
        todo.CreatedAt = DateTime.Now;

        await _todoRepository.AddTodoAsync(todo);

        return Ok(todo);
    }

    [Authorize]
    [HttpDelete]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Delete(int id)
    {
        await _todoRepository.DeleteTodoAsync(id);

        return Ok("todo has been deleted successfully");
    }

    [Authorize]
    [HttpPut]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Edit(int id, Todo editedTodo)
    {
        var todo = await _todoRepository.GetTodoByIdAsync(id);

        if (todo == null)
        {
            _logger.LogWarning("##############=> todo is not found");
            return NotFound();
        }
        // TODO: check this line below : it exists also in the repo 
        editedTodo.UpdatedAt = DateTime.Now;

        await _todoRepository.EditTodoAsync(id, editedTodo);
            
        return Ok(todo);
    }
}

