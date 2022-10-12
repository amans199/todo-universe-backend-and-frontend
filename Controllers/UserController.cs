using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using todo_universe.Manager;
using todo_universe.Models;
using todo_universe.Data;

namespace todo_universe.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly JwtAuthenticationManager jwtAuthenticationManager;
        public readonly AppDbContext _dbContext;
        public LoginController(JwtAuthenticationManager jwtAuthenticationManager,AppDbContext dbContext)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            _dbContext = dbContext;
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult Authorize([FromBody] User usr)
        {
            var userData = _dbContext.Users.FirstOrDefault(u => u.UserName == usr.UserName);

            if (userData == null)
                return Unauthorized();

            var token = jwtAuthenticationManager.Authenticate(usr.UserName, usr.Password);
            if (string.IsNullOrEmpty(token))
                return Unauthorized();


            return Ok(new { token, userData });
        }
    }
}