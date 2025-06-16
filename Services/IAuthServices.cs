using APIs_Graduation.Models;

namespace APIs_Graduation.Services
{
    public interface IAuthServices
    {

        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}
