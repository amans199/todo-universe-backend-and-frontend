using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_universe.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Avatar { get; set; } = String.Empty;
        public string? Email { get; set; } = String.Empty;
        public string? FirstName { get; set; } = String.Empty;
        public string? LastName { get; set; } = String.Empty;
        public string? Role { get; set; } = "user";

    }
}