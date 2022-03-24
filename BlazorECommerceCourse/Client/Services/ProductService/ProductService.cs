using System.Net.Http.Json;

namespace BlazorECommerceCourse.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public List<Product> Products { get; set; } = new();

    public async Task<ServiceResponse<Product>> GetProduct(int productId)
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product?>>($"api/product/{productId}");
        return result;
    }

    public async Task GetProducts()
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");
        if (result is not null && result.Data is not null)
            Products = result.Data;
    }
}
