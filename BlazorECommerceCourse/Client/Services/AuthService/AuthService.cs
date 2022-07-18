﻿using System.Net.Http.Json;

namespace BlazorECommerceCourse.Client.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
