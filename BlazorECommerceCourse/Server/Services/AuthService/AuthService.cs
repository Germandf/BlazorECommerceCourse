﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BlazorECommerceCourse.Server.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        DataContext dataContext,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = dataContext;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<string>> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
        
        if (user is null)
            return new() { Success = false, Message = "User not found" };

        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return new() { Success = false, Message = "Wrong password" };

        var jwtToken = CreateToken(user);
        return new() { Success = true, Data = jwtToken };
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

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:JwtKey").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user is null)
            return new() { Success = false, Message = "User not found." };

        CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        await _context.SaveChangesAsync();
        return new() { Success = true, Message = "Password has been changed." };
    }

    public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public string GetUserEmail() => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)!;

    public async Task<User?> GetUserByEmail(string email) => await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
}
