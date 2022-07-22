using BlazorECommerceCourse.Server.Services.CartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorECommerceCourse.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("products")]
    public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = await _cartService.GetCartProducts(cartItems);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreCartItems(List<CartItem> cartItems)
    {
        var result = await _cartService.StoreCartItems(cartItems);
        return Ok(result);
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> AddToCart(CartItem cartItem)
    {
        var result = await _cartService.AddToCart(cartItem);
        return Ok(result);
    }

    [HttpPut("update-quantity")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuantity(CartItem cartItem)
    {
        var result = await _cartService.UpdateQuantity(cartItem);
        return Ok(result);
    }

    [HttpDelete("{productId}/{productTypeId}")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<bool>>> RemoveItemFromCart(int productId, int productTypeId)
    {
        var result = await _cartService.RemoveItemFromCart(productId, productTypeId);
        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()
    {
        return await _cartService.GetCartItemsCount();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetStoredCartProducts()
    {
        var result = await _cartService.GetStoredCartProducts();
        return Ok(result);
    }
}
