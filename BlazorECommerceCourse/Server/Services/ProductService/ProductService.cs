namespace BlazorECommerceCourse.Server.Services.ProductService;

public class ProductService : IProductService
{
    private readonly DataContext _context;

    public ProductService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
    {
        var response = new ServiceResponse<List<Product>>
        {
            Success = true,
            Data = await _context.Products.Where(x => x.Featured)
                .Include(x => x.Variants)
                .ToListAsync()
        };
        return response;
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

    public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
    {
        var products = await FindProductsBySearchText(searchText);
        var results = new List<string>();

        foreach(var product in products)
        {
            if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                results.Add(product.Title);
            if (product.Description is not null)
            {
                var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
                var words = product.Description.Split().Select(x => x.Trim(punctuation));
                foreach (var word in words)
                    if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !results.Contains(word))
                        results.Add(word);
            }
        }

        var response = new ServiceResponse<List<string>>()
        {
            Success = true,
            Data = results
        };
        return response;
    }

    public async Task<ServiceResponse<ProductSearchResultDto>> SearchProducts(string searchText, int page)
    {
        var pageResults = 2;
        var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / (float)pageResults);
        var products = await _context.Products
                    .Where(x => x.Title.ToLower().Contains(searchText.ToLower()) ||
                                x.Description.ToLower().Contains(searchText.ToLower()))
                    .Include(x => x.Variants)
                    .Skip((page - 1) * pageResults)
                    .Take(pageResults)
                    .ToListAsync();

        var response = new ServiceResponse<ProductSearchResultDto>()
        {
            Success = true,
            Data = new()
            {
                Products = products,
                CurrentPage = page,
                Pages = (int)pageCount
            }
        };
        return response;
    }

    private async Task<List<Product>> FindProductsBySearchText(string searchText)
    {
        return await _context.Products
                    .Where(x => x.Title.ToLower().Contains(searchText.ToLower()) ||
                                x.Description.ToLower().Contains(searchText.ToLower()))
                    .Include(x => x.Variants)
                    .ToListAsync();
    }
}
