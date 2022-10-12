using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using todo_universe.Models;
using todo_universe.Data;

namespace todo_universe.Manager
{
    public class JwtAuthenticationManager
    {
        //key declaration
        private readonly IConfiguration _configuration;

        //cant  consume smaller life time service in a longer lifetime service => so we cant get dbContext in here ..
           // scoped service cant be used in singeleton
        public JwtAuthenticationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? Authenticate(string username, string password)
        {
            
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_configuration["Jwt:Token"]);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                // Duration of the Token
                // Now the the Duration to 1 Hour
                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature) //setting sha256 algorithm
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}