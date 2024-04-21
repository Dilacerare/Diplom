using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;

namespace Diplom.Service.Interfaces;

public interface IProfileService
{
    Task<BaseResponse<ProfileViewModel>> GetProfile(string userName);
        
    Task<BaseResponse<ProfileViewModel>> Save(ProfileViewModel model);

    
}