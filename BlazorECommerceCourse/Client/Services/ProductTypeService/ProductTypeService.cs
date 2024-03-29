﻿using System.Net.Http.Json;

namespace BlazorECommerceCourse.Client.Services.ProductTypeService;

public class ProductTypeService : IProductTypeService
{
    private readonly HttpClient _httpClient;

    public ProductTypeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public List<ProductType> ProductTypes { get; set; } = new();

    public event Action? OnChange;

    public async Task AddProductType(ProductType productType)
    {
        var response = await _httpClient.PostAsJsonAsync("api/producttype", productType);
        ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>())?.Data!;
        OnChange?.Invoke();
    }

    public ProductType CreateNewProductType()
    {
        var newProductType = new ProductType() { IsNew = true, Editing = true };
        ProductTypes.Add(newProductType);
        OnChange?.Invoke();
        return newProductType;
    }

    public async Task GetProductTypes()
    {
        var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/producttype");
        ProductTypes = result?.Data ?? new();
    }

    public async Task UpdateProductType(ProductType productType)
    {
        var response = await _httpClient.PutAsJsonAsync("api/producttype", productType);
        ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>())?.Data!;
        OnChange?.Invoke();
    }
}
