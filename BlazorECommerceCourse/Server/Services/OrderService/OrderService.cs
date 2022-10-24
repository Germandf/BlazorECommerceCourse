namespace BlazorECommerceCourse.Server.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly DataContext _context;
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;

    public OrderService(
        DataContext context,
        ICartService cartService,
        IAuthService authService)
    {
        _context = context;
        _cartService = cartService;
        _authService = authService;
    }

    public async Task<ServiceResponse<bool>> PlaceOrder()
    {
        var products = (await _cartService.GetStoredCartProducts()).Data;

        if (products is null)
            return new() { Success = false, Data = false, Message = "User has no products in cart." };

        decimal totalPrice = 0;
        var orderItems = new List<OrderItem>();

        foreach (var product in products)
        {
            totalPrice += product.Price * product.Quantity;
            orderItems.Add(new() { ProductId = product.ProductId, ProductTypeId = product.ProductTypeId, 
                Quantity = product.Quantity, TotalPrice = product.Price * product.Quantity });
        }

        var order = new Order() { 
            UserId = _authService.GetUserId(), OrderDate = DateTime.Now, TotalPrice = totalPrice, OrderItems = orderItems };

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(_context.CartItems.Where(x => x.UserId == _authService.GetUserId()));
        await _context.SaveChangesAsync();
        return new() { Success = true, Data = true };
    }
}
