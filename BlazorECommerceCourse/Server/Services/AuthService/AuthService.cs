using System.Security.Cryptography;
using System.Text;

namespace BlazorECommerceCourse.Server.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly DataContext _context;

    public AuthService(DataContext dataContext)
    {
        _context = dataContext;
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        if (await UserExists(user.Email))
            return new() { Success = false, Message = "User already exists" };

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new() { Success = true, Data = user.Id, Message = "Registration successful" };
    }

    public async Task<bool> UserExists(string email)
    {
        if (await _context.Users.AnyAsync(x => x.Email.ToLower().Equals(email.ToLower())))
            return true;

        return false;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
