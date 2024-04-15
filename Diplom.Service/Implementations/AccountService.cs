using System.Security.Claims;
using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Helpers;
using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;
using Diplom.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Diplom.Service.Implementations;

public class AccountService : IAccountService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly ILogger<AccountService> _logger;
    
    public AccountService(IBaseRepository<User> userRepository, ILogger<AccountService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == model.Login);
            if (user == null)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = "Пользователь не найден"
                };
            }

            if (user.Password != HashPasswordHelper.HashPassword(model.Password))
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = "Неверный пароль или логин"
                };
            }
            var result = Authenticate(user);

            return new BaseResponse<ClaimsIdentity>()
            {
                Data = result,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[Login]: {ex.Message}");
            return new BaseResponse<ClaimsIdentity>()
            {
                Description = ex.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public Task<BaseResponse<bool>> ChangePassword(ChangePasswordViewModel model)
    {
        throw new NotImplementedException();
    }
    
    private ClaimsIdentity Authenticate(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
        };
        return new ClaimsIdentity(claims, "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
    }
}