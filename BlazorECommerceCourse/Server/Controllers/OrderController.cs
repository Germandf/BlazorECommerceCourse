using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorECommerceCourse.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<bool>>> PlaceOrder()
    {
        var result = await _orderService.PlaceOrder();
        return Ok(result);
    }
}
