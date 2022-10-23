namespace todo_universe.Models
{
    public interface ICategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
