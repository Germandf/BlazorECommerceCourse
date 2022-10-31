global using BlazorECommerceCourse.Client;
global using BlazorECommerceCourse.Client.Services.AuthService;
global using BlazorECommerceCourse.Client.Services.CartService;
global using BlazorECommerceCourse.Client.Services.CategoryService;
global using BlazorECommerceCourse.Client.Services.OrderService;
global using BlazorECommerceCourse.Client.Services.ProductService;
global using BlazorECommerceCourse.Client.Services.AddressService;
global using BlazorECommerceCourse.Client.Services.ProductTypeService;
global using BlazorECommerceCourse.Shared;
global using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();
