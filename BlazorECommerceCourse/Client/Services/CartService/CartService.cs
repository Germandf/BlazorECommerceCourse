using Blazored.LocalStorage;

namespace BlazorECommerceCourse.Client.Services.CartService;

public class CartService : ICartService
{
    ILocalStorageService _localStorage;

    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public event Action OnChange;

    public async Task AddToCart(CartItem cartItem)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart is null)
            cart = new();
        cart.Add(cartItem);
        await _localStorage.SetItemAsync("cart", cart);
        OnChange?.Invoke();
    }

    public async Task<List<CartItem>> GetCartItems()
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart is null)
            cart = new();
        return cart;
    }
}
