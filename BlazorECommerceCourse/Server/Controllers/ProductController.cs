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
}
