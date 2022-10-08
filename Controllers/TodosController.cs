using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using todo_universe.Data;
using todo_universe.Models;

namespace todo_universe.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TodosController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var todos = _dbContext.Todos;

            return Ok(todos);
        }

        [HttpPost]
        public IActionResult Add(Todo todo)
        {
            _dbContext.Todos.Add(todo);
            _dbContext.SaveChanges();

            return Ok(todo);
        }

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

        [HttpPut]
        public async Task<IActionResult> Edit(int id, Todo editedTodo)
        {
            var todo = _dbContext.Todos.Find(id);

            if (todo == null)
                return NotFound();

            //todo = editedTodo;


            _dbContext.Entry(todo).CurrentValues.SetValues(editedTodo);

            await _dbContext.SaveChangesAsync();

            return Ok(todo);
        }
    }
}
