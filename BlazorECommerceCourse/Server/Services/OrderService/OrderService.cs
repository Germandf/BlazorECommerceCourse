using System.Security.Claims;

namespace BlazorECommerceCourse.Server.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly DataContext _context;
    private readonly ICartService _cartService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrderService(
        DataContext context, 
        ICartService cartService, 
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _cartService = cartService;
        _httpContextAccessor = httpContextAccessor;
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

        var order = new Order() { UserId = GetUserId(), OrderDate = DateTime.Now, TotalPrice = totalPrice, OrderItems = orderItems };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return new() { Success = true, Data = true };
    }

    private int GetUserId() =>
        int.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
