using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_universe.Models
{
    public class Todo : ITodo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = String.Empty;
        public bool IsComplete { get; set; } = false;

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public int? UserId { get; set; }
    }
}
