using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;
using Diplom.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Diplom.Service.Implementations;

public class AccessPermissionService : IAccessPermissionService
{
    private readonly IBaseRepository<AccessPermission> _accessRepository;
    private readonly ILogger<AccountService> _logger;
    
    public AccessPermissionService(IBaseRepository<AccessPermission> accessRepository, ILogger<AccountService> logger)
    {
        _accessRepository = accessRepository;
        _logger = logger;
    }
    
    public async Task<IBaseResponse<AccessPermissionViewModel>> Create(AccessPermissionViewModel model)
    {
        try
        {
            AccessPermission? access = await _accessRepository.GetAll().FirstOrDefaultAsync(x => 
                x.Initiator == model.Initiator 
                && x.Addressee == model.Addressee && 
                (x.Status == AccessStatus.Requested || x.Status == AccessStatus.Allowed)
                );
            if (access != null)
            {
                return new BaseResponse<AccessPermissionViewModel>()
                {
                    Description = "Такой запрос рассматривается или уже одобрен",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            
            access = new AccessPermission()
            {
                Initiator = model.Initiator,
                Addressee = model.Addressee,
                Status = model.Status,
                Date = model.Date
            };
            
            await _accessRepository.Create(access);
            
            return new BaseResponse<AccessPermissionViewModel>()
            {
                Data = model,
                Description = "Запрос отправлен",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[AccessPermissionService.Create] error: {ex.Message}");
            return new BaseResponse<AccessPermissionViewModel>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
}