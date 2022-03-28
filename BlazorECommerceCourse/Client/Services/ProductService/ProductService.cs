using System.Net.Http.Json;

namespace BlazorECommerceCourse.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public event Action ProductsChanged;
    public List<Product> Products { get; set; } = new();
    public string Message { get; set; } = "Loading products...";

    public async Task<ServiceResponse<Product>> GetProduct(int productId)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product?>>($"api/product/{productId}");
        return result;
    }

    public async Task GetProducts(string? categoryUrl = null)
    {
        var result = categoryUrl is null ?
            await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured") :
            await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
        if (result is not null && result.Data is not null)
            Products = result.Data;
        ProductsChanged?.Invoke();
    }

    public async Task<List<string>> GetProductSearchSuggestions(string searchText)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");
        if (result is not null && result.Data is not null)
            return result.Data;
        else
            return new();
    }

    public async Task SearchProducts(string searchText)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/search/{searchText}");
        if (result is not null && result.Data is not null)
            Products = result.Data;
        if (!Products.Any())
            Message = "No products found.";
        ProductsChanged?.Invoke();
    }
}
