
namespace ORS.Service.Contracts
{
    public interface IUserService
    {
        Task AddUserAsync(string email, string password);
        Task<bool> AuthenticateUserAsync(string email, string password);
        string GenerateJwtToken(string username, IList<string> roles);
    }
}