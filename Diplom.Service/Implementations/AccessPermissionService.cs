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

    public async Task<IBaseResponse<IEnumerable<AccessPermissionViewModel>>> GetAccesses(string login)
    {
        var accesses = _accessRepository.GetAll()
            .Where(x => x.Addressee == login || x.Initiator == login)
            .Select(x => new AccessPermissionViewModel()
            {
                Addressee = x.Addressee,
                Initiator = x.Initiator,
                Date = x.Date,
                Status = x.Status
            });
        
        if (accesses == null)
        {
            return new BaseResponse<IEnumerable<AccessPermissionViewModel>>()
            {
                Description = "Уведомлений нет",
                StatusCode = StatusCode.OK
            };
        }
        
        return new BaseResponse<IEnumerable<AccessPermissionViewModel>>()
        {
            Data = accesses,
            StatusCode = StatusCode.OK
        };
    }

    public async Task<IBaseResponse<AccessPermissionViewModel>> Allow(AccessPermissionViewModel model)
    {
        try
        {
            var access = await _accessRepository.GetAll().FirstOrDefaultAsync(x => 
                x.Initiator == model.Initiator && 
                x.Addressee == model.Addressee && 
                x.Status == model.Status
            );
            if (access == null)
            {
                return new BaseResponse<AccessPermissionViewModel>()
                {
                    Description = "Что-то пошло не так",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            access.Status = AccessStatus.Allowed;
            
            await _accessRepository.Update(access);
            
            return new BaseResponse<AccessPermissionViewModel>()
            {
                Data = model,
                Description = "Запрос разрешен",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[AccessPermissionService.Allow] error: {ex.Message}");
            return new BaseResponse<AccessPermissionViewModel>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
    
    public async Task<IBaseResponse<AccessPermissionViewModel>> Refuse(AccessPermissionViewModel model)
    {
        try
        {
            var access = await _accessRepository.GetAll().FirstOrDefaultAsync(x => 
                x.Initiator == model.Initiator && 
                x.Addressee == model.Addressee && 
                x.Status == model.Status
            );
            if (access == null)
            {
                return new BaseResponse<AccessPermissionViewModel>()
                {
                    Description = "Что-то пошло не так",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            access.Status = AccessStatus.Refused;
            
            await _accessRepository.Update(access);
            
            return new BaseResponse<AccessPermissionViewModel>()
            {
                Data = model,
                Description = "Запрос откланён",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[AccessPermissionService.Refuse] error: {ex.Message}");
            return new BaseResponse<AccessPermissionViewModel>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
    
    public async Task<BaseResponse<bool>> NotificationStatus(string login)
    {
        try
        {
            var access = await _accessRepository.GetAll().FirstOrDefaultAsync(x => 
                x.Addressee == login && x.Status == AccessStatus.Requested);
            
            if (access == null)
            {
                return new BaseResponse<bool>()
                {
                    Data = false,
                    Description = "Уведомлений нет",
                    StatusCode = StatusCode.OK
                };
            }
            
            return new BaseResponse<bool>()
            {
                Data = true,
                Description = "Уведомления есть",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[AccessPermissionService.NotificationStatus] error: {ex.Message}");
            return new BaseResponse<bool>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
}