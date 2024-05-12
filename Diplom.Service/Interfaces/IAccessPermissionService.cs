using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;

namespace Diplom.Service.Interfaces;

public interface IAccessPermissionService
{
    Task<IBaseResponse<AccessPermissionViewModel>> Create(AccessPermissionViewModel model);

    Task<IBaseResponse<IEnumerable<AccessPermissionViewModel>>> GetAccesses(string login);

    Task<IBaseResponse<AccessPermissionViewModel>> Allow(AccessPermissionViewModel model);

    Task<IBaseResponse<AccessPermissionViewModel>> Refuse(AccessPermissionViewModel model);

    Task<BaseResponse<bool>> NotificationStatus(string login);

}