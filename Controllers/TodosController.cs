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

        //private IHttpContextAccessor _httpContextAccessor;

        public TodosController(ILogger<TodosController> logger, AppDbContext dbContext, JwtAuthenticationManager jwtAuthenticationManager)
        {
            _jwtAuthManager = jwtAuthenticationManager;
            _dbContext = dbContext;
            _logger = logger;
            //_httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string? title = null, int? id = null,bool? isComplete = null)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userName = _jwtAuthManager.GetUserName(token);
            if(userName ==null)
            {
                return Unauthorized();
            }
            var userId = GetUserId(userName);

            
            _logger.LogInformation("##############=> getting all todos");
            var todos = _dbContext.Todos.Where(t => t.UserId == userId).AsQueryable();

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

            return Ok(todos);
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
            var userId = GetUserId(userName);
            
            todo.UserId = userId;
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
            var userId = GetUserId(userName);

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
            var userId = GetUserId(userName);

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

            _dbContext.Todos.Update(todo);
            await _dbContext.SaveChangesAsync();

            return Ok(todo);
        }

        public int GetUserId(string userName)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            return user.Id;
        }
    }
}
