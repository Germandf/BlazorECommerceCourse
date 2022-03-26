namespace BlazorECommerceCourse.Server.Services.ProductService;

public class ProductService : IProductService
{
    private readonly DataContext _context;

    public ProductService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<Product?>> GetProduct(int productId)
    {
        var product = await _context.Products
            .Include(x => x.Variants)
            .ThenInclude(x => x.ProductType)
            .FirstOrDefaultAsync(x => x.Id == productId);
        if(product is null)
            return new() { Success = false, Message = "The requested product does not exist." };
        return new() { Success = true, Data = product };
    }

    public async Task<ServiceResponse<List<Product>>> GetProducts()
    {
        var response = new ServiceResponse<List<Product>>()
        {
            Success = true,
            Data = await _context.Products
                .Include(x => x.Variants)
                .ToListAsync()
        };
        return response;
    }

    public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
    {
        var response = new ServiceResponse<List<Product>>()
        {
            Success = true,
            Data = await _context.Products
                .Where(x => x.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                .Include(x => x.Variants)
                .ToListAsync()
        };
        return response;
    }
}
