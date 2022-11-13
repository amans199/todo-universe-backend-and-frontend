using todo_universe.Models;

namespace todo_universe.Repository;
public interface ICategoryRepository
{
    public Task<Category> AddCategoryAsync(Category todo);
    public Task<Category> DeleteCategoryAsync(int id);
    public Task<Category> EditCategoryAsync(int id, Category editedCategory);
    public Task<Category> GetCategoryByIdAsync(int id);
    public Task<List<Category>> GetCategoriesAsync(int userId);
}
