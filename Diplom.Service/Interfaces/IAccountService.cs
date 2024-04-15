using System.Security.Claims;
using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;

namespace Diplom.Service.Interfaces;

public interface IAccountService
{
    Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model);

    Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);

    Task<BaseResponse<bool>> ChangePassword(ChangePasswordViewModel model);
}