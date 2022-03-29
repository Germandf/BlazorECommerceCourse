namespace BlazorECommerceCourse.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;

    public CartService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<CartProductDto>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = new ServiceResponse<List<CartProductDto>>()
        {
            Success = true,
            Data = new()
        };
        foreach(var cartItem in cartItems)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cartItem.ProductId);
            if (product is null)
                continue;
            var productVariant = await _context.ProductVariants
                .AsNoTracking()
                .Include(x => x.ProductType)
                .FirstOrDefaultAsync(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);
            if (productVariant is null)
                continue;
            var cartProduct = new CartProductDto()
            {
                ProductId = product.Id,
                Title = product.Title,
                ImageUrl = product.ImageUrl,
                Price = productVariant.Price,
                ProductType = productVariant.ProductType.Name,
                ProductTypeId = productVariant.ProductTypeId,
                Quantity = cartItem.Quantity,
            };
            result.Data.Add(cartProduct);
        }
        return result;
    }
}
