using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Extensions;
using Diplom.Domain.Helpers;
using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;
using Diplom.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Diplom.Service.Implementations;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IBaseRepository<User> _userRepository;

    public UserService(ILogger<UserService> logger, IBaseRepository<User> userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<IBaseResponse<User>> Create(UserViewModel model)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == model.Login);
            if (user != null)
            {
                return new BaseResponse<User>()
                {
                    Description = "Пользователь с таким логином уже есть",
                    StatusCode = StatusCode.UserAlreadyExists
                };
            }
            user = new User()
            {
                Login = model.Login,
                Role = Enum.Parse<Role>(model.Role),
                Password = HashPasswordHelper.HashPassword(model.Password),
            };
            
            await _userRepository.Create(user);
            
            return new BaseResponse<User>()
            {
                Data = user,
                Description = "Пользователь добавлен",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService.Create] error: {ex.Message}");
            return new BaseResponse<User>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }

    public BaseResponse<Dictionary<int, string>> GetRoles()
    {
        try
        {
            var roles = ((Role[]) Enum.GetValues(typeof(Role)))
                .ToDictionary(k => (int) k, t => t.GetDisplayName());

            return new BaseResponse<Dictionary<int, string>>()
            {
                Data = roles,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<Dictionary<int, string>>()
            {
                Description = ex.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    
    public async Task<BaseResponse<IEnumerable<UserViewModel>>> GetUsers()
    {
        try
        {
            var users = await _userRepository.GetAll()
                .Select(x => new UserViewModel()
                {
                    Id = x.Id,
                    Login = x.Login,
                    Role = x.Role.GetDisplayName()
                })
                .ToListAsync();

            _logger.LogInformation($"[UserService.GetUsers] получено элементов {users.Count}");
            return new BaseResponse<IEnumerable<UserViewModel>>()
            {
                Data = users,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserSerivce.GetUsers] error: {ex.Message}");
            return new BaseResponse<IEnumerable<UserViewModel>>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }

    public async Task<IBaseResponse<UserViewModel>> GetUser(long id)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var data = new UserViewModel()
            {
                Role = user.Role.GetDisplayName(),
                Login = user.Login,
                Password = user.Password
            };

            return new BaseResponse<UserViewModel>()
            {
                StatusCode = StatusCode.OK,
                Data = data
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<UserViewModel>()
            {
                Description = $"[GetHealth] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<bool>> DeleteUser(long id)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCode.UserNotFound,
                    Data = false
                };
            }
            await _userRepository.Delete(user);
            _logger.LogInformation($"[UserService.DeleteUser] пользователь удален");
            
            return new BaseResponse<bool>
            {
                StatusCode = StatusCode.OK,
                Data = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserSerivce.DeleteUser] error: {ex.Message}");
            return new BaseResponse<bool>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }

    public async Task<IBaseResponse<User>> Edit(long id, UserViewModel model)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return new BaseResponse<User>()
                {
                    Description = "User not found",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            Role role = Role.Patient;
            switch (model.Role)
            {
                case "Пациент":
                    role = Role.Patient;
                    break;
                case "Врач":
                    role = Role.Doctor;
                    break;
                case "Админ":
                    role = Role.Admin;
                    break;
                default:
                    return new BaseResponse<User>()
                    {
                        Description = "Role not found",
                        StatusCode = StatusCode.UserNotFound
                    };
            }

            user.Login = model.Login;
            user.Role = role;
            user.Password = model.Password;


            await _userRepository.Update(user);


            return new BaseResponse<User>()
            {
                Data = user,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<User>()
            {
                Description = $"[Edit] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}