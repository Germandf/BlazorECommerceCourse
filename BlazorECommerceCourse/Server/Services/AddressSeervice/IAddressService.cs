namespace BlazorECommerceCourse.Server.Services.AddressSeervice;

public interface IAddressService
{
    Task<ServiceResponse<Address>> GetAddress();
    Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address);
}
