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

    public async Task GetCategories()
    {
        var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");
        if (response is not null && response.Data is not null)
            Categories = response.Data;
    }
}
