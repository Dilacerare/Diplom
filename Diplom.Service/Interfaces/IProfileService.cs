using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;

namespace Diplom.Service.Interfaces;

public interface IProfileService
{
    Task<BaseResponse<ProfileViewModel>> GetProfile(string userName);
    
    Task<BaseResponse<ProfileAccessViewModel>> GetProfileAccess(string initiator, string addressee);
        
    Task<BaseResponse<ProfileViewModel>> Save(ProfileViewModel model);
    
    Task<BaseResponse<ProfileViewModel>> AddMedReport(MedicalReportViewModel model);

    Task<BaseResponse<PatientRegisterViewModel>> AddNewPatient(PatientRegisterViewModel model);
}