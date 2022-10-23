using Microsoft.EntityFrameworkCore;
using todo_universe.Data;
using todo_universe.Manager;
using todo_universe.Models;

namespace todo_universe.Helpers
{
    public class UserHelpers
    {

        public readonly AppDbContext _dbContext;

        public UserHelpers(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public User GetUserData(User user)
        {
            var userData = _dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);

            return userData;
        }
    }
}
