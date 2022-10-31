using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Authentication;
using todo_universe.Data;
using todo_universe.Models;
using todo_universe.Manager;

namespace todo_universe.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TodosController> _logger;
        private readonly JwtAuthenticationManager _jwtAuthManager;

        public TodosController(ILogger<TodosController> logger, AppDbContext dbContext, JwtAuthenticationManager jwtAuthenticationManager)
        {
            _jwtAuthManager = jwtAuthenticationManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string? title = null, int? id = null,bool? isComplete = null, int? categoryId = null)
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

            var todosWithCategories = todos.Select(todo => new
            {
                todo.Id,
                todo.Title,
                todo.CreatedAt,
                todo.UpdatedAt,
                //todo.Description,
                todo.IsComplete,
                todo.RemindAt,
                todo.CategoryId,
                Category = categories.FirstOrDefault(c => c.Id == todo.CategoryId)
            });

            return Ok(todosWithCategories);
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
            _dbContext.Todos.Add(todo);
            _dbContext.SaveChanges();

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

            var todo = _dbContext.Todos.Find(id);

            if(todo == null) 
                return NotFound();

            if (todo.UserId != userId)
                return Unauthorized();

            _dbContext.Todos.Remove(todo);
            _dbContext.SaveChanges();

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

            todo.Title = editedTodo.Title;
            todo.IsComplete = editedTodo.IsComplete;
            todo.CategoryId = editedTodo.CategoryId;
            todo.RemindAt = editedTodo.RemindAt;
            todo.UpdatedAt = DateTime.Now;

            _dbContext.Todos.Update(todo);
            await _dbContext.SaveChangesAsync();

            return Ok(todo);
        }
    }
}
