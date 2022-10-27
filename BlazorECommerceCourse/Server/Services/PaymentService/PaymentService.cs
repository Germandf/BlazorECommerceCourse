using Stripe;
using Stripe.Checkout;

namespace BlazorECommerceCourse.Server.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;
    private readonly IOrderService _orderService;

    string? _secret = Environment.GetEnvironmentVariable("StripeCliKey");

    public PaymentService(
        ICartService cartService, 
        IAuthService authService, 
        IOrderService orderService)
    {
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripeApiKey");
        _cartService = cartService;
        _authService = authService;
        _orderService = orderService;
    }

    public async Task<Session> CreateCheckoutSession()
    {
        var products = (await _cartService.GetStoredCartProducts()).Data;
        var lineItems = new List<SessionLineItemOptions>();

        products?.ForEach(product => lineItems.Add(new SessionLineItemOptions()
        {
            PriceData = new SessionLineItemPriceDataOptions()
            {
                UnitAmountDecimal = product.Price * 100,
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions()
                {
                    Name = product.Title,
                    Images = new List<string>() { product.ImageUrl }
                }
            },
            Quantity = product.Quantity
        }));

        var options = new SessionCreateOptions()
        {
            CustomerEmail = _authService.GetUserEmail(),
            PaymentMethodTypes = new List<string>()
            {
                "card"
            },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = "https://localhost:7154/order-success",
            CancelUrl = "https://localhost:7154/cart"
        };

        var service = new SessionService();
        var session = service.Create(options);
        return session;
    }

    public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest httpRequest)
    {
        var json = await new StreamReader(httpRequest.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, httpRequest.Headers["Stripe-Signature"], _secret);

            if (stripeEvent.Type != Events.CheckoutSessionCompleted)
                return new ServiceResponse<bool>() { Data = false, Success = false, Message = "Payment is not completed" };

            var session = stripeEvent.Data.Object as Session;
            var user = await _authService.GetUserByEmail(session?.CustomerEmail ?? "");
            var success = await _orderService.PlaceOrder(user?.Id ?? 0);
            return new ServiceResponse<bool>() { Data = success.Data, Success = success.Data };
        }
        catch (StripeException e)
        {
            return new ServiceResponse<bool>() { Data = false, Success = false, Message = e.Message };
        }
    }
}
