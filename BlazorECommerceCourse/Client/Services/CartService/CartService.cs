using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace BlazorECommerceCourse.Client.Services.CartService;

public class CartService : ICartService
{
    public event Action? OnChange;

    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CartService(
        ILocalStorageService localStorage, 
        HttpClient httpClient, 
        AuthenticationStateProvider authenticationStateProvider)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task AddToCart(CartItem cartItem)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        if (authState.User.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            Console.WriteLine("User is authenticated");
        }
        else
        {
            Console.WriteLine("User is not authenticated");
        }

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

    public async Task<List<CartProductResponse>> GetCartProducts()
    {
        var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
        var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
        return cartProducts?.Data ?? new();
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
        OnChange?.Invoke();
    }

    public async Task UpdateQuantity(CartProductResponse product)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        
        if (cart is null)
            return;
        
        var cartItem = cart.FirstOrDefault(
            x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);
        
        if (cartItem is null)
            return;
        
        cartItem.Quantity = product.Quantity;
        await _localStorage.SetItemAsync("cart", cart);
    }

    public async Task StoreCartItems(bool emptyLocalCart = false)
    {
        var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        
        if (localCart is null)
            return;

        await _httpClient.PostAsJsonAsync("api/cart", localCart);

        if (emptyLocalCart)
            await _localStorage.RemoveItemAsync("cart");
    }
}
