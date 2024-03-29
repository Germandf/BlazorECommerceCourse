﻿using System.Net.Http.Json;

namespace BlazorECommerceCourse.Client.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
    }

    public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
    {
        var result = await _httpClient.PostAsJsonAsync("api/auth/change-password", request.Password);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>() ??
            new() { Success = false, Message = "Unknown error, please try again later" };
    }

    public async Task<bool> IsUserAuthenticated()
    {
        return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity?.IsAuthenticated ?? false;
    }

    public async Task<ServiceResponse<string>> Login(UserLogin request)
    {
        var result = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>() ??
            new() { Success = false, Message = "Unknown error, please try again later" };
    }

    public async Task<ServiceResponse<int>> Register(UserRegister request)
    {
        var result = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>() ?? 
            new() { Success = false, Message = "Unknown error, please try again later" };
    }
}
