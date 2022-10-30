namespace BlazorECommerceCourse.Server.Services.AddressSeervice;

public class AddressService : IAddressService
{
    private readonly DataContext _context;
    private readonly IAuthService _authService;

    public AddressService(DataContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
    {
        var response = new ServiceResponse<Address>();
        var dbAddress = (await GetAddress()).Data;
        if (dbAddress is null)
        {
            address.UserId = _authService.GetUserId();
            _context.Addresses.Add(address);
        }
        else
        {
            dbAddress.FirstName = address.FirstName;
            dbAddress.LastName = address.LastName;
            dbAddress.State = address.State;
            dbAddress.Country = address.Country;
            dbAddress.City = address.City;
            dbAddress.Zip = address.Zip;
            dbAddress.Street = address.Street;
        }
        response.Data = address;
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<ServiceResponse<Address>> GetAddress()
    {
        var userId = _authService.GetUserId();
        var address = await _context.Addresses.FirstOrDefaultAsync(x => x.UserId == userId);
        return new ServiceResponse<Address>() { Data = address };
    }
}
