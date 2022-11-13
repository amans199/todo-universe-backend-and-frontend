using System.ComponentModel.DataAnnotations;

namespace todo_universe.Models;

public interface IUser
{
    //[Key]
    public int Id { get; set; }
    //public Guid Guid { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
}
