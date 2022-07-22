using Microsoft.AspNetCore.Components;

namespace BlazorECommerceCourse.Client.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager _navigationManager;

    public OrderService(
        HttpClient httpClient, 
        AuthenticationStateProvider authStateProvider, 
        NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task PlaceOrder()
    {
        if (await UserIsAuthenticated())
        {
            await _httpClient.PostAsync("api/order", null);
        }
        else
        {
            _navigationManager.NavigateTo("login");
        }
    }

    private async Task<bool> UserIsAuthenticated()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity is not null && authState.User.Identity.IsAuthenticated)
            return true;

        return false;
    }
}
