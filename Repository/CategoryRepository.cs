using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todo_universe.Data;
using todo_universe.Models;

namespace todo_universe.Repository;
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<Category> DeleteCategoryAsync(int id)
    {
        var category = await _dbContext.Categories.FindAsync(id);

        if (category == null)
        {
            return null;
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<Category> EditCategoryAsync(int id, Category editedCategory)
    {
        var category = await _dbContext.Categories.FindAsync(id);

        if (category == null) return null;

        category.Name = editedCategory.Name;
        //category.UpdatedAt = DateTime.Now;

        _dbContext.Entry(category).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        return editedCategory;
    }

// todo get categories by user id 
    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _dbContext.Categories.FindAsync(id);
    }

    public async Task<List<Category>> GetCategoriesAsync(int userId)
    {
        return await _dbContext.Categories.Where(t => t.UserId == userId).ToListAsync();
    }

    //public async Task<Category> AddTodoToCategoryAsync(int categoryId, Todo todo)
    //{
    //    var category = await _dbContext.Categories.FindAsync(categoryId);

    //    if (category == null) return null;

    //    category.Todos.Add(todo);
    //    await _dbContext.SaveChangesAsync();
    //    return category;
    //}

    //public async Task<Category> DeleteTodoFromCategoryAsync(int categoryId, int todoId)
    //{
    //    var category = await _dbContext.Categories.FindAsync(categoryId);

    //    if (category == null) return null;

    //    var todo = await _dbContext.Todos.FindAsync(todoId);

    //    if (todo == null) return null;

    //    category.Todos.Remove(todo);
    //    await _dbContext.SaveChangesAsync();
    //    return category;
    //}
}
