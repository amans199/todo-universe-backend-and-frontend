using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using todo_universe.Manager;
using todo_universe.Models;
using todo_universe.Data;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using todo_universe.Helpers;
using todo_universe.Services;

namespace todo_universe.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly JwtAuthenticationManager _jwtAuthManager;
        public readonly AppDbContext _dbContext;
        public readonly AccountService _accountService;

        public UserController(JwtAuthenticationManager jwtAuthenticationManager, AppDbContext dbContext, AccountService accountService)
        {
            _jwtAuthManager = jwtAuthenticationManager;
            _dbContext = dbContext;
            _accountService = accountService;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] User user)
        {
            var userData = _dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);

            if (userData == null)
                return Unauthorized();

            string hashedPassword = _accountService.HashPassword(user.Password, userData.Salt);
            if (userData.Password != hashedPassword)
                return Unauthorized();

            var token = _jwtAuthManager.Authenticate(user.UserName);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var tokenExp = DateTime.UtcNow.AddHours(1);

            return Ok(new { token, tokenExp, userData });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register([FromBody] User user)
        {
            var userData = _dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);

            if (userData != null)
                return BadRequest();


            string generatedSalt = _accountService.GenerateSalt();
            string hashedPassword = _accountService.HashPassword(user.Password, generatedSalt);
            _dbContext.Users.Add(new User { UserName = user.UserName, Password = hashedPassword, Salt = generatedSalt });
            _dbContext.SaveChanges();

            var token = _jwtAuthManager.Authenticate(user.UserName);

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var tokenExp = DateTime.UtcNow.AddHours(1);

            return Ok(new { token, tokenExp, userData= user });
        }


        [HttpPost]
        [Authorize]
        public IActionResult Account([FromBody] String userName)
        {
            var userData = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);

            return Ok(userData);
        }


        [HttpPut]
        [Authorize]
        public IActionResult Update([FromBody] User user)
        {
            var userData = _dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);

            if (userData == null)
                return BadRequest();

            userData.Password = user.Password;
            userData.Email = user.Email;
            userData.FirstName = user.FirstName;
            userData.LastName = user.LastName;
            userData.Avatar = user.Avatar;

            _dbContext.Users.Update(userData);

            _dbContext.SaveChanges();

            return Ok(userData);
        }
    }
}