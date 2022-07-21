using System.Security.Claims;

namespace BlazorECommerceCourse.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public CartService(
        DataContext context, 
        IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
    }

    public async Task<ServiceResponse<int>> GetCartItemsCount()
    {
        var cartItems = await _context.CartItems.Where(x => x.UserId == GetUserId()).ToListAsync();
        return new() { Success = true, Data = cartItems.Count };
    }

    public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = new ServiceResponse<List<CartProductResponse>>()
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
            
            var cartProduct = new CartProductResponse()
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

    public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
    {
        var nullableUserId = GetUserId();
        if (nullableUserId is not int userId)
            return new() { Success = false, Message = "User not found" };

        cartItems.ForEach(x => x.UserId = userId);
        _context.CartItems.AddRange(cartItems);
        await _context.SaveChangesAsync();

        return await GetCartProducts(
            await _context.CartItems.Where(x => x.UserId == userId).ToListAsync());
    }

    private int? GetUserId()
    {
        try
        {
            return int.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        catch
        {
            return null;
        }
    }
        
}
