namespace BlazorECommerceCourse.Shared;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public Address Address { get; set; } = null!;
    public string Role { get; set; } = "Customer";
}
