namespace BlazorECommerceCourse.Server.Services.CartService;

public interface ICartService
{
    Task<ServiceResponse<List<CartProductDto>>> GetCartProducts(List<CartItem> cartItems);
}
