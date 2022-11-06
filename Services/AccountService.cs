using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace todo_universe.Services;
public class AccountService
{

    public string HashPassword(string password, string salt = null)
    {
        // we can also use  PasswordHasher from microsoft identity
        // refer to : https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1?view=aspnetcore-6.0#methods
        
        if (salt == null)
        {
            salt = GenerateSalt();
        }

        var saltedPassword = password + salt;
        var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
        var hash = SHA256.Create().ComputeHash(saltedPasswordBytes);
        var hashedPassword = Convert.ToBase64String(hash);
        return hashedPassword;
    }

    public string GenerateSalt()
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }


    //public string HashPassword(string password)
    //{


    //        //best way to save the salt is in 
    //    using var hmac = new HMACSHA512();
    //    var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    //    var passwordSalt = hmac.Key;
    //    return Convert.ToBase64String(passwordHash) + ":" + Convert.ToBase64String(passwordSalt);
    //}
}
