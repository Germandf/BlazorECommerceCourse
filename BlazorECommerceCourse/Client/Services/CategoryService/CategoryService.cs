using System.Net.Http.Json;

namespace BlazorECommerceCourse.Client.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public List<Category> Categories { get; set; } = new();
    public List<Category> AdminCategories { get; set; } = new();

    public event Action? OnChange;

    public async Task AddCategory(Category category)
    {
        var response = await _httpClient.PostAsJsonAsync("api/category/admin", category);
        AdminCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>())?.Data!;
        await GetCategories();
        OnChange?.Invoke();
    }

    public Category CreateNewCategory()
    {
        var newCategory = new Category() { IsNew = true, Editing = true };
        AdminCategories.Add(newCategory);
        OnChange?.Invoke();
        return newCategory;
    }

    public async Task DeleteCategory(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/category/admin/{id}");
        AdminCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>())?.Data!;
        await GetCategories();
        OnChange?.Invoke();
    }

    public async Task GetAdminCategories()
    {
        var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category/admin");
        if (response is not null && response.Data is not null)
            AdminCategories = response.Data;
    }

    public async Task GetCategories()
    {
        var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");
        if (response is not null && response.Data is not null)
            Categories = response.Data;
    }

    public async Task UpdateCategory(Category category)
    {
        var response = await _httpClient.PutAsJsonAsync("api/category/admin", category);
        AdminCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>())?.Data!;
        await GetCategories();
        OnChange?.Invoke();
    }
}
