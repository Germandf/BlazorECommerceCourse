namespace BlazorECommerceCourse.Client.Services.ProductTypeService;

public interface IProductTypeService
{
    event Action? OnChange;
    List<ProductType> ProductTypes { get; set; }
    Task GetProductTypes();
    Task AddProductType(ProductType productType);
    Task UpdateProductType(ProductType productType);
    ProductType CreateNewProductType();
}