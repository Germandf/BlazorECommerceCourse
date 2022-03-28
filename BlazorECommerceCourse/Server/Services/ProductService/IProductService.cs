namespace BlazorECommerceCourse.Server.Services.ProductService;

public interface IProductService
{
    Task<ServiceResponse<List<Product>>> GetProducts();
    Task<ServiceResponse<Product?>> GetProduct(int productId);
    Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl);
    Task<ServiceResponse<ProductSearchResultDto>> SearchProducts(string searchText, int page);
    Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText);
    Task<ServiceResponse<List<Product>>> GetFeaturedProducts();
}
