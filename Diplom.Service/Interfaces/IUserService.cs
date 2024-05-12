using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;

namespace Diplom.Service.Interfaces;

public interface IUserService
{
    Task<IBaseResponse<User>> Create(UserViewModel model);
        
    BaseResponse<Dictionary<int, string>> GetRoles();
        
    Task<BaseResponse<IEnumerable<UserViewModel>>> GetUsers(Role role);
        
    Task<IBaseResponse<UserViewModel>> GetUser(string login);
        
    Task<IBaseResponse<bool>> DeleteUser(long id);
        
    Task<IBaseResponse<User>> Edit(long id, UserViewModel model);
}