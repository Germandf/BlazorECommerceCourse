using Stripe.Checkout;

namespace BlazorECommerceCourse.Server.Services.PaymentService;

public interface IPaymentService
{
    Task<Session> CreateCheckoutSession();
    Task<ServiceResponse<bool>> FulfillOrder(HttpRequest httpRequest);
}
