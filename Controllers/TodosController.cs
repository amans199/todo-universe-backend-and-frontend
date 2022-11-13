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
using TodoUniverse.Library;
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
    private readonly TodoService _todoService;
    
    public TodosController(ILogger<TodosController> logger, AppDbContext dbContext, JwtAuthenticationManager jwtAuthenticationManager, ITodoRepository todoRepository, TodoService todoService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _jwtAuthManager = jwtAuthenticationManager;
        _todoRepository = todoRepository;
        _todoService = todoService;
    }

    [Authorize]
    [HttpGet]
    //[ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Index([FromQuery] string? title, [FromQuery] int? id, [FromQuery] bool? isComplete,[FromQuery] int? categoryId,[FromQuery] int? orderByTitle,[FromQuery] int? orderByCreatedAt,[FromQuery] int? orderByUpdatedAt,[FromQuery] int? orderByRemindAt)
    {
    //{
        var userIdtest = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    //    if (userId == null)
    //    {
    //        throw new AuthenticationException("User not found");
    //    }

        var userName = User.FindFirstValue(ClaimTypes.Name);

        var userId =  _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

        if (userId == null)
        {
            return Unauthorized();
        }


        _logger.LogInformation("##############=> getting all todos");

        //string? title, int? id, bool? isComplete, int? categoryId, int? orderByTitle, int? orderByCreatedAt, int? orderByUpdatedAt, int? orderByRemindAt
        //userId,
        List<Todo> todos = _todoService.GetAllTodosAsync( title, id, isComplete, categoryId, orderByTitle, orderByCreatedAt, orderByUpdatedAt, orderByRemindAt).Result.ToList();
        //var todos = _dbContext.Todos.Where(t => t.UserId == userId).AsQueryable();
        var categories = _dbContext.Categories.Where(t => t.UserId == userId).AsQueryable();
        //if (!String.IsNullOrEmpty(title))
        //{
        //    todos = todos.Where(todo => todo.Title.Contains(title));
        //}

        //if (id != null || id is not null)
        //{
        //    todos = todos.Where(todo => todo.Id ==id);
        //}

        //if (isComplete != null)
        //{
        //    todos = todos.Where(todo => todo.IsComplete == isComplete);
        //}

        //if (categoryId != null)
        //{
        //    todos = todos.Where(todo => todo.CategoryId == categoryId);
        //}

        //if (orderByTitle == 1)
        //{
        //    todos = todos.OrderBy(todo => todo.Title);
        //}
        //else if (orderByTitle == 2)
        //{
        //    todos = todos.OrderByDescending(todo => todo.Title);
        //}

        //if (orderByCreatedAt == 1)
        //{
        //    todos = todos.OrderBy(todo => todo.CreatedAt);
        //}
        //else if (orderByCreatedAt == 2)
        //{
        //    todos = todos.OrderByDescending(todo => todo.CreatedAt);
        //}

        //if (orderByUpdatedAt == 1)
        //{
        //    todos = todos.OrderBy(todo => todo.UpdatedAt);
        //}
        //else if (orderByUpdatedAt == 2)
        //{
        //    todos = todos.OrderByDescending(todo => todo.UpdatedAt);
        //}

        //if (orderByRemindAt == 1)
        //{
        //    todos = todos.OrderBy(todo => todo.RemindAt);
        //}
        //else if (orderByRemindAt == 2)
        //{
        //    todos = todos.OrderByDescending(todo => todo.RemindAt);
        //}


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
        var userName = User.FindFirstValue(ClaimTypes.Name);

        var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

        if (userId == null)
        {
            return Unauthorized();
        }

        todo.UserId = userId;
        todo.CreatedAt = DateTime.Now;

        await _todoRepository.AddTodoAsync(todo);

        return Ok(todo);
    }

    [Authorize]
    [HttpDelete]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Delete(int id)
    {

        var userName = User.FindFirstValue(ClaimTypes.Name);

        var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

        if (userId == null)
        {
            return Unauthorized();
        }

        await _todoRepository.DeleteTodoAsync(id);

        return Ok("todo has been deleted successfully");
    }

    [Authorize]
    [HttpPut]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Edit(int id, Todo editedTodo)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);

        var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

        if (userId == null)
        {
            return Unauthorized();
        }

        var todo = _dbContext.Todos.Find(id);

        if (todo == null)
        {
            _logger.LogWarning("##############=> todo is not found");
            return NotFound();
        }

        if (todo.UserId != userId)
        {
            _logger.LogWarning("##############=> todo is not found");
            return Unauthorized();
        }

        // TODO: check this line below : it exists also in the repo 
        editedTodo.UpdatedAt = DateTime.Now;

        await _todoRepository.EditTodoAsync(id, editedTodo);
            
        return Ok(todo);
    }
}

