namespace BlazorECommerceCourse.Server.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly DataContext _context;

    public CategoryService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Category>>> AddCategory(Category category)
    {
        category.Editing = category.IsNew = false;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return await GetAdminCategories();
    }

    public async Task<ServiceResponse<List<Category>>> DeleteCategory(int id)
    {
        var category = await GetCategoryById(id);

        if (category is null)
            return new ServiceResponse<List<Category>>() { Success = false, Message = "Category not found." };

        category.Deleted = true;
        await _context.SaveChangesAsync();
        return await GetAdminCategories();
    }

    private async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ServiceResponse<List<Category>>> GetAdminCategories()
    {
        var categories = await _context.Categories.Where(x => !x.Deleted).ToListAsync();
        return new ServiceResponse<List<Category>>() { Data = categories };
    }

    public async Task<ServiceResponse<List<Category>>> GetCategories()
    {
        var categories = await _context.Categories.Where(x => !x.Deleted && x.Visible).ToListAsync();
        return new ServiceResponse<List<Category>>() { Data = categories };
    }

    public async Task<ServiceResponse<List<Category>>> UpdateCategory(Category category)
    {
        var dbCategory = await GetCategoryById(category.Id);

        if (dbCategory is null)
            return new ServiceResponse<List<Category>>() { Success = false, Message = "Category not found." };

        dbCategory.Name = category.Name;
        dbCategory.Url = category.Url;
        dbCategory.Visible = category.Visible;

        await _context.SaveChangesAsync();
        return await GetAdminCategories();
    }
}
