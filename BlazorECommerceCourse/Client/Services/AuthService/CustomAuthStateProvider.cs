using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorECommerceCourse.Client.Services.AuthService;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(
        ILocalStorageService localStorageService, 
        HttpClient httpClient)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var authToken = await _localStorageService.GetItemAsStringAsync("authToken");
        var identity = new ClaimsIdentity();
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(authToken))
        {
            try
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
            }
            catch
            {
                await _localStorageService.RemoveItemAsync("authToken");
                identity = new ClaimsIdentity();
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split(".")[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var kvPairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        var claims = kvPairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
        return claims ?? new List<Claim>();
    }
}
