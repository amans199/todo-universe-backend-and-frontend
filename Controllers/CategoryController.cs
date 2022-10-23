using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using todo_universe.Data;
using todo_universe.Manager;
using todo_universe.Models;

namespace todo_universe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CategoryController> _logger;
        private readonly JwtAuthenticationManager _jwtAuthManager;

        public CategoryController(ILogger<CategoryController> logger, AppDbContext dbContext, JwtAuthenticationManager jwtAuthenticationManager)
        {
            _jwtAuthManager = jwtAuthenticationManager;
            _dbContext = dbContext;
            _logger = logger;
        }


        [Authorize]
        [HttpGet]
        public IActionResult Index(string? name = null, int? id = null,string? color=null, string? description=null)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userName = _jwtAuthManager.GetUserName(token);
            if (userName == null)
            {
                return Unauthorized();
            }
            var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;


            _logger.LogInformation("##############=> getting all categories");

            var categories = _dbContext.Categories.Where(t => t.UserId == userId).AsQueryable();

            if (!String.IsNullOrEmpty(name))
            {
                categories = categories.Where(category  => category.Name.Contains(name));
            }

            if (id != null || id is not null)
            {
                categories = categories.Where(category => category.Id == id);
            }

            if (!String.IsNullOrEmpty(color))
            {
                categories = categories.Where(category => category.Color == color);
            }

            if (!String.IsNullOrEmpty(description))
            {
                categories = categories.Where(category => category.Description.Contains(description));
            }

            return Ok(categories);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(Category category)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userName = _jwtAuthManager.GetUserName(token);
            if (userName == null)
            {
                return Unauthorized();
            }
            
            var userId = _dbContext.Users.FirstOrDefault(u => u.UserName == userName)?.Id;

            if(userId == null)
            {
                return Unauthorized();
            }

            category.UserId = (int)userId;
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            return Ok(category);
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

            var category = _dbContext.Categories.Find(id);

            if (category == null)
                return NotFound();

            if (category.UserId != userId)
                return Unauthorized();

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            return Ok("category has been deleted successfully");
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Edit(int id, Category editedCategory)
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

            var category = _dbContext.Categories.Find(id);

            if (category == null)
            {
                _logger.LogWarning("##############=> category is not found");
                return NotFound();
            }

            if (category.UserId != userId)
            {
                _logger.LogWarning("##############=> category is not found");
                return Unauthorized();
            }

            category.Name = editedCategory.Name;
            category.Description = editedCategory.Description;
            category.Color = editedCategory.Color;

            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();

            return Ok(category);
        }
    }
}
