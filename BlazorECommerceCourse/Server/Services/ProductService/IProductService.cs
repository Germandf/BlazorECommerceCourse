namespace BlazorECommerceCourse.Server.Services.ProductService;

public interface IProductService
{
    Task<ServiceResponse<List<Product>>> GetProducts();
}
