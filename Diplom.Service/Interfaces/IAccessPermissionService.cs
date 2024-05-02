using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;

namespace Diplom.Service.Interfaces;

public interface IAccessPermissionService
{
    Task<IBaseResponse<AccessPermissionViewModel>> Create(AccessPermissionViewModel model);
    
}