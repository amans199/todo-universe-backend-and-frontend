using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using todo_universe.Data;
using todo_universe.Models;

namespace todo_universe.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        //private readonly JwtAuthManager _jwtAuthManager;
        private readonly AppDbContext _dbContext;

        public TodosController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string? title = null, int? id = null,bool? isComplete = null)
        {
            var todos = _dbContext.Todos.AsQueryable();

            if (!String.IsNullOrEmpty(title))
            {
                todos = todos.Where(todo => todo.Title.Contains(title));
            }

            if (id != null || id is not null)
            {
                todos = todos.Where(todo => todo.Id ==id);
            }

            if(isComplete != null)
            {
                todos = todos.Where(todo => todo.IsComplete == isComplete);
            }


            var token = Request.Headers["authorization"].FirstOrDefault()?.Split(" ").Last();
            //if (token != null && _jwtAuthManager.ValidateToken(token))
            //{
                return Ok(todos);
            //}
            //return Unauthorized();

        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(Todo todo)
        {
            _dbContext.Todos.Add(todo);
            _dbContext.SaveChanges();

            return Ok(todo);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var todo = _dbContext.Todos.Find(id);

            if(todo == null) 
                return NotFound();

            _dbContext.Todos.Remove(todo);
            _dbContext.SaveChanges();

            return Ok("todo has been deleted successfully");
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Edit(int id, Todo editedTodo)
        {
            var todo = _dbContext.Todos.Find(id);

            if (todo == null)
                return NotFound();

            todo.Title = editedTodo.Title;
            todo.IsComplete = editedTodo.IsComplete;

            _dbContext.Todos.Update(todo);
            await _dbContext.SaveChangesAsync();

            return Ok(todo);

            //var todo = _dbContext.Todos.Find(id);

            //if (todo == null)
            //    return NotFound();

            ////todo = editedTodo;


            //_dbContext.Entry(todo).CurrentValues.SetValues(editedTodo);

            //await _dbContext.SaveChangesAsync();

            //return Ok(todo);
        }
    }
}
