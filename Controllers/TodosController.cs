using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Authentication;
using todo_universe.Data;
using todo_universe.Models;
using todo_universe.Manager;
using todo_universe.Repository;

namespace todo_universe.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TodosController> _logger;
        private readonly JwtAuthenticationManager _jwtAuthManager;
        private readonly ITodoRepository _todoRepository;

        public TodosController(ILogger<TodosController> logger, AppDbContext dbContext, JwtAuthenticationManager jwtAuthenticationManager, ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
            _jwtAuthManager = jwtAuthenticationManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string? title = null, int? id = null,bool? isComplete = null, int? categoryId = null,int orderByTitle = 0, int orderByCreatedAt = 0, int orderByUpdatedAt = 0, int orderByRemindAt = 0)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userName = _jwtAuthManager.GetUserName(token);
            if(userName ==null)
            {
                return Unauthorized();
            }
            var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

            if (userId == null)
            {
                return Unauthorized();
            }


            _logger.LogInformation("##############=> getting all todos");

            var todos = _dbContext.Todos.Where(t => t.UserId == userId).AsQueryable();
            var categories = _dbContext.Categories.Where(t => t.UserId == userId).AsQueryable();
            if (!String.IsNullOrEmpty(title))
            {
                todos = todos.Where(todo => todo.Title.Contains(title));
            }

            if (id != null || id is not null)
            {
                todos = todos.Where(todo => todo.Id ==id);
            }

            if (isComplete != null)
            {
                todos = todos.Where(todo => todo.IsComplete == isComplete);
            }

            if (categoryId != null)
            {
                todos = todos.Where(todo => todo.CategoryId == categoryId);
            }

            if (orderByTitle == 1)
            {
                todos = todos.OrderBy(todo => todo.Title);
            }
            else if (orderByTitle == 2)
            {
                todos = todos.OrderByDescending(todo => todo.Title);
            }

            if (orderByCreatedAt == 1)
            {
                todos = todos.OrderBy(todo => todo.CreatedAt);
            }
            else if (orderByCreatedAt == 2)
            {
                todos = todos.OrderByDescending(todo => todo.CreatedAt);
            }

            if (orderByUpdatedAt == 1)
            {
                todos = todos.OrderBy(todo => todo.UpdatedAt);
            }
            else if (orderByUpdatedAt == 2)
            {
                todos = todos.OrderByDescending(todo => todo.UpdatedAt);
            }

            if (orderByRemindAt == 1)
            {
                todos = todos.OrderBy(todo => todo.RemindAt);
            }
            else if (orderByRemindAt == 2)
            {
                todos = todos.OrderByDescending(todo => todo.RemindAt);
            }


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
        public IActionResult Add(Todo todo)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userName = _jwtAuthManager.GetUserName(token);
            if (userName == null)
            {
                return Unauthorized();
            }
            var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

            if (userId == null)
            {
                return Unauthorized();
            }

            todo.UserId = userId;
            todo.CreatedAt = DateTime.Now;

            _todoRepository.AddTodoAsync(todo);

            return Ok(todo);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userName = _jwtAuthManager.GetUserName(token);
            if (userName == null)
            {
                return Unauthorized();
            }
            var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

            if (userId == null)
            {
                return Unauthorized();
            }

            _todoRepository.DeleteTodoAsync(id);

            return Ok("todo has been deleted successfully");
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Edit(int id, Todo editedTodo)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userName = _jwtAuthManager.GetUserName(token);
            if (userName == null)
            {
                return Unauthorized();
            }
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

            //todo.Title = editedTodo.Title;
            //todo.IsComplete = editedTodo.IsComplete;
            //todo.CategoryId = editedTodo.CategoryId;
            //todo.RemindAt = editedTodo.RemindAt;
            editedTodo.UpdatedAt = DateTime.Now;

            //_dbContext.Todos.Update(todo);
            //await _dbContext.SaveChangesAsync();

            await _todoRepository.EditTodoAsync(id, editedTodo);
            
            return Ok(todo);
        }
    }
}
