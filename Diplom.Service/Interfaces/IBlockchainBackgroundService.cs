using Diplom.BlockchainApp.Entity;
using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.Service.Interfaces;

public interface IBlockchainBackgroundService
{
    void AddToBlockchain(Blockchain blockchain, IBaseRepository<Buffer> _bufferRepository, IBaseRepository<User> _userRepository);
}