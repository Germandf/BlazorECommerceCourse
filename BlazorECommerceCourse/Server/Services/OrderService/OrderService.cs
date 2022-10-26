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

    public async Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId)
    {
        var response = new ServiceResponse<OrderDetailsResponse>();

        var order = await _context.Orders
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.Product)
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.ProductType)
            .Where(x => x.UserId == _authService.GetUserId() && x.Id == orderId)
            .OrderByDescending(x => x.OrderDate)
            .FirstOrDefaultAsync();

        if (order is null)
        {
            response.Success = false;
            response.Message = "Order not found.";
            return response;
        }

        var orderDetailResponse = new OrderDetailsResponse()
        {
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Products = new()
        };

        order.OrderItems.ForEach(x => orderDetailResponse.Products.Add(new()
        {
            ProductId = x.ProductId,
            ImageUrl = x.Product.ImageUrl,
            ProductType = x.ProductType.Name,
            Quantity = x.Quantity,
            Title = x.Product.Title,
            TotalPrice = x.TotalPrice
        }));

        response.Data = orderDetailResponse;
        return response;
    }

    public async Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrders()
    {
        var response = new ServiceResponse<List<OrderOverviewResponse>>();

        var orders = await _context.Orders
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.Product)
            .Where(x => x.UserId == _authService.GetUserId())
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();

        var orderResponses = new List<OrderOverviewResponse>();

        orders.ForEach(x => orderResponses.Add(new OrderOverviewResponse()
        {
            Id = x.Id,
            OrderDate = x.OrderDate,
            TotalPrice = x.TotalPrice,
            Product = x.OrderItems.Count > 1 ? 
                $"{x.OrderItems.First().Product.Title} and {x.OrderItems.Count - 1} more..." :
                x.OrderItems.First().Product.Title,
            ProductImageUrl = x.OrderItems.First().Product.ImageUrl
        }));

        response.Data = orderResponses;
        return response;
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
