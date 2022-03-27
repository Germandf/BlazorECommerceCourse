using Microsoft.AspNetCore.Mvc;

namespace BlazorECommerceCourse.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
    {
        var response = await _productService.GetProducts();
        return Ok(response);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int productId)
    {
        var response = await _productService.GetProduct(productId);
        return Ok(response);
    }

    [HttpGet("category/{categoryUrl}")]
    public async Task<ActionResult<ServiceResponse<Product>>> GetProductsByCategory(string categoryUrl)
    {
        var response = await _productService.GetProductsByCategory(categoryUrl);
        return Ok(response);
    }

    [HttpGet("search/{searchText}")]
    public async Task<ActionResult<ServiceResponse<Product>>> SearchProducts(string searchText)
    {
        var response = await _productService.SearchProducts(searchText);
        return Ok(response);
    }

    [HttpGet("searchsuggestions/{searchText}")]
    public async Task<ActionResult<ServiceResponse<string>>> GetProductSearchSuggestions(string searchText)
    {
        var response = await _productService.GetProductSearchSuggestions(searchText);
        return Ok(response);
    }
}
