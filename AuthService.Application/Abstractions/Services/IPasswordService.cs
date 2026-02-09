namespace AuthService.Application.Abstractions.Services
{
    public interface IPasswordService
    {
        string Hash(string plainPassword);
        bool Verify(string plainPassword, string passwordHash);
    }
}
