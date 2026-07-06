namespace AuthService.Application.Abstractions.Security
{
    public interface ICommonPasswordChecker
    {
        bool IsCommon(string password);
    }
}
