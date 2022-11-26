namespace BlazorECommerceCourse.Server.Services.ProductService;

public class ProductService : IProductService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccesor;

    public ProductService(DataContext context, IHttpContextAccessor httpContextAccesor)
    {
        _context = context;
        _httpContextAccesor = httpContextAccesor;
    }

    public async Task<ServiceResponse<Product>> CreateProduct(Product product)
    {
        foreach (var variant in product.Variants)
            variant.ProductType = null!;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return new() { Success = true, Data = product };
    }

    public async Task<ServiceResponse<bool>> DeleteProduct(int productId)
    {
        var dbProduct = await _context.Products.FindAsync(productId);
        if (dbProduct is null)
            return new() { Success = false, Data = false, Message = "Product not found" };
        dbProduct.Deleted = true;
        await _context.SaveChangesAsync();
        return new() { Success = true, Data = true };
    }

    public async Task<ServiceResponse<List<Product>>> GetAdminProducts()
    {
        var response = new ServiceResponse<List<Product>>()
        {
            Success = true,
            Data = await _context.Products
                .Where(x => !x.Deleted)
                .Include(x => x.Variants.Where(y => !y.Deleted))
                .ThenInclude(x => x.ProductType)
                .Include(x => x.Images)
                .ToListAsync()
        };
        return response;
    }

    public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
    {
        var response = new ServiceResponse<List<Product>>
        {
            Success = true,
            Data = await _context.Products
                .Where(x => x.Featured && x.Visible && !x.Deleted)
                .Include(x => x.Variants.Where(y => y.Visible && !y.Deleted))
                .Include(x => x.Images)
                .ToListAsync()
        };
        return response;
    }

    public async Task<ServiceResponse<Product?>> GetProduct(int productId)
    {
        var isAdmin = _httpContextAccesor.HttpContext?.User.IsInRole("Admin") ?? false;
        var product = await _context.Products
            .Include(x => x.Variants.Where(y => !y.Deleted && (isAdmin || y.Visible)))
            .ThenInclude(x => x.ProductType)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == productId && !x.Deleted && (isAdmin || x.Visible));
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
                .Where(x => x.Visible && !x.Deleted)
                .Include(x => x.Variants.Where(y => y.Visible && !y.Deleted))
                .Include(x => x.Images)
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
                .Where(x => x.Category.Url.ToLower().Equals(categoryUrl.ToLower()) && x.Visible && !x.Deleted)
                .Include(x => x.Variants.Where(y => y.Visible && !y.Deleted))
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
                    .Where(x => (x.Title.ToLower().Contains(searchText.ToLower()) ||
                        x.Description.ToLower().Contains(searchText.ToLower())) &&
                        x.Visible && !x.Deleted)
                    .Include(x => x.Variants.Where(y => y.Visible && !y.Deleted))
                    .Include(x => x.Images)
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

    public async Task<ServiceResponse<Product>> UpdateProduct(Product product)
    {
        var dbProduct = await _context.Products
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == product.Id);
        if (dbProduct is null)
            return new() { Success = false, Message = "Product not found" };
        dbProduct.Title = product.Title;
        dbProduct.Description = product.Description;
        dbProduct.ImageUrl = product.ImageUrl;
        dbProduct.CategoryId = product.CategoryId;
        dbProduct.Visible = product.Visible;
        dbProduct.Featured = product.Featured;
        _context.Images.RemoveRange(dbProduct.Images);
        dbProduct.Images = product.Images;
        foreach (var variant in product.Variants)
        {
            var dbVariant = await _context.ProductVariants.SingleOrDefaultAsync(x => 
                x.ProductId == variant.ProductId && x.ProductTypeId == variant.ProductTypeId);
            if (dbVariant is null)
            {
                variant.ProductType = null!;
                _context.ProductVariants.Add(variant);
            }
            else
            {
                dbVariant.ProductTypeId = variant.ProductTypeId;
                dbVariant.Price = variant.Price;
                dbVariant.OriginalPrice = variant.OriginalPrice;
                dbVariant.Visible = variant.Visible;
                dbVariant.Deleted = variant.Deleted;
            }
        }
        await _context.SaveChangesAsync();
        return new() { Success = true, Data = product };
    }

    private async Task<List<Product>> FindProductsBySearchText(string searchText)
    {
        return await _context.Products
                    .Where(x => (x.Title.ToLower().Contains(searchText.ToLower()) ||
                        x.Description.ToLower().Contains(searchText.ToLower())) && 
                        x.Visible && !x.Deleted)
                    .Include(x => x.Variants.Where(y => y.Visible && !y.Deleted))
                    .ToListAsync();
    }
}
