using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Response;
using Diplom.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.Service.Implementations;

public class BufferService : IBufferService
{
    private readonly ILogger<BufferService> _logger;
    private readonly IBaseRepository<Buffer> _bufferRepository;

    public BufferService(ILogger<BufferService> logger, IBaseRepository<Buffer> bufferRepository)
    {
        _logger = logger;
        _bufferRepository = bufferRepository;
    }
    
    public async Task<IBaseResponse<Buffer>> Create(string data, string login, string addressee, string operationName)
    {
        try
        {
            var buffer = new Buffer()
            {
                Login = login,
                Addressee = addressee,
                OperationName = operationName,
                Data = data
            };

            await _bufferRepository.Create(buffer);

            return new BaseResponse<Buffer>()
            {
                Data = buffer,
                Description = "Пользователь добавлен",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[BufferService.Create] error: {ex.Message}");
            return new BaseResponse<Buffer>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }

    public Task<BaseResponse<IEnumerable<Buffer>>> GetBuffer()
    {
        throw new NotImplementedException();
    }

    public async Task<IBaseResponse<bool>> DeleteBuffer(long id)
    {
        try
        {
            var buffer = await _bufferRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (buffer == null)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Data = false
                };
            }
            await _bufferRepository.Delete(buffer);
            _logger.LogInformation($"[BufferService.DeleteBuffer] буффер удален");
            
            return new BaseResponse<bool>
            {
                StatusCode = StatusCode.OK,
                Data = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[BufferService.DeleteBuffer] error: {ex.Message}");
            return new BaseResponse<bool>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
}