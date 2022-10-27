namespace BlazorECommerceCourse.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;
    private readonly IAuthService _authService;

    public CartService(
        DataContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
    {
        var userId = _authService.GetUserId();
        cartItem.UserId = userId;
        var dbCartItem = await GetCartItem(cartItem);

        if (dbCartItem is null)
            _context.CartItems.Add(cartItem);
        else
            dbCartItem.Quantity += cartItem.Quantity;

        await _context.SaveChangesAsync();
        return new() { Success = true, Data = true };
    }

    

    public async Task<ServiceResponse<int>> GetCartItemsCount()
    {
        var cartItems = await _context.CartItems.Where(x => x.UserId == _authService.GetUserId()).ToListAsync();
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

    public async Task<ServiceResponse<List<CartProductResponse>>> GetStoredCartProducts(int? userId = null)
    {
        if (userId is null)
            userId = _authService.GetUserId();

        var userCartItems = await _context.CartItems.Where(x => x.UserId == userId).ToListAsync();
        return await GetCartProducts(userCartItems);
    }

    public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
    {
        var userId = _authService.GetUserId();
        var dbCartItem = await _context.CartItems.FirstOrDefaultAsync(
            x => x.ProductId == productId && x.ProductTypeId == productTypeId && x.UserId == userId);

        if (dbCartItem is null)
            return new() { Success = false, Data = false, Message = "Cart item does not exist." };

        _context.CartItems.Remove(dbCartItem);
        await _context.SaveChangesAsync();

        return new() { Success = true, Data = true };
    }

    public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
    {
        var userId = _authService.GetUserId();
        cartItems.ForEach(x => x.UserId = userId);
        _context.CartItems.AddRange(cartItems);
        await _context.SaveChangesAsync();

        return await GetStoredCartProducts();
    }

    public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem)
    {
        var userId = _authService.GetUserId();
        cartItem.UserId = userId;
        var dbCartItem = await GetCartItem(cartItem);

        if (dbCartItem is null)
            return new() { Success = false, Data = false, Message = "Cart item does not exist." };

        dbCartItem.Quantity = cartItem.Quantity;
        await _context.SaveChangesAsync();
        return new() { Success = true, Data = true };
    }

    private async Task<CartItem?> GetCartItem(CartItem cartItem) =>
        await _context.CartItems.FirstOrDefaultAsync(x =>
            x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId && x.UserId == cartItem.UserId);
}
