using Diplom.Domain.Entity;
using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.Service.Interfaces;

public interface IBufferService
{
    Task<IBaseResponse<Buffer>> Create(string data, string login, string addressee, string operationName);
    
    Task<BaseResponse<IEnumerable<Buffer>>> GetBuffer();
    
    Task<IBaseResponse<bool>> DeleteBuffer(long id);
}