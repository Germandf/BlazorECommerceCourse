using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace BlazorECommerceCourse.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public CartService(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public event Action OnChange;

    public async Task AddToCart(CartItem cartItem)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart is null)
            cart = new();
        var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);
        if (sameItem is null)
            cart.Add(cartItem);
        else
            sameItem.Quantity += cartItem.Quantity;
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

    public async Task<List<CartProductDto>> GetCartProducts()
    {
        var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
        var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductDto>>>();
        return cartProducts.Data;
    }

    public async Task RemoveProductFromCart(int productId, int productTypeId)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart is null)
            return;
        var cartItem = cart.FirstOrDefault(x => x.ProductId == productId && x.ProductTypeId == productTypeId);
        if (cartItem is null)
            return;
        cart.Remove(cartItem);
        await _localStorage.SetItemAsync("cart", cart);
        OnChange.Invoke();
    }

    public async Task UpdateQuantity(CartProductDto product)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart is null)
            return;
        var cartItem = cart.FirstOrDefault(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);
        if (cartItem is null)
            return;
        cartItem.Quantity = product.Quantity;
        await _localStorage.SetItemAsync("cart", cart);
    }
}
