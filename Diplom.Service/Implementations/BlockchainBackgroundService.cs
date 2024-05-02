using Diplom.BlockchainApp.Entity;
using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;
using Diplom.Domain.Helpers;
using Diplom.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.Service.Implementations;

public class BlockchainBackgroundService : BackgroundService, IBlockchainBackgroundService
{
    private readonly ILogger<BlockchainBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BlockchainBackgroundService(ILogger<BlockchainBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Blockchain blockchain = BlockchainFileManager.LoadBlockchain("..\\Diplom.Blockchain\\Source\\blockchain.json");
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
                var bufferRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Buffer>>();
                AddToBlockchain(blockchain, bufferRepository, userRepository);
                // AddToBlockchain(blockchain, scope);
            }
            // AddToBlockchain(blockchain);

            // Подождать 5 секунд перед следующим вызовом
            await Task.Delay(10000, stoppingToken);
        }
    }

    public async void AddToBlockchain(Blockchain blockchain,IBaseRepository<Buffer> _bufferRepository, IBaseRepository<User> _userRepository)
    {
        var buffers = _bufferRepository.GetAll();
        
        if (!buffers.Any())
            return;
        
        
        foreach (var buffer in buffers)
        {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == buffer.Addressee);
                CryptographyHelper.Encrypt(buffer.Data, user.PublickKey, out var encryptedData, out var encryptedKey, out var encryptedIv);
                var dataBlock = new DataBlock(buffer.Login, buffer.Addressee, buffer.OperationName, encryptedKey, encryptedIv, encryptedData);
                blockchain.AddBlock(new Block(blockchain.GetLatestBlock().Index + 1, new DateTime(),
                    JsonConvert.SerializeObject(dataBlock), blockchain.GetLatestBlock().Hash));
                user.HashCode = blockchain.GetLatestBlock().Hash;
                BlockchainFileManager.SaveBlockchain(blockchain, "..\\Diplom.Blockchain\\Source\\blockchain.json");
                await _userRepository.Update(user);
        
                await _bufferRepository.Delete(buffer);
        }
    }
    
    public async void AddToBlockchain(Blockchain blockchain, IServiceScope scope)
    {
        
        var _userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
        var _bufferRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Buffer>>();
        
        var buffers = _bufferRepository.GetAll();
        
        if (!buffers.Any())
            return;
        
        
        foreach (var buffer in buffers)
        {
        //     using (var scope = _serviceScopeFactory.CreateScope())
        //     {
        //         var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == buffer.Login);
        //         CryptographyHelper.Encrypt(buffer.Data, user.PublickKey, out var encryptedData, out var encryptedKey, out var encryptedIv);
        //         var dataBlock = new DataBlock(buffer.Login, buffer.Addressee, buffer.OperationName, encryptedKey, encryptedIv, encryptedData);
        //         blockchain.AddBlock(new Block(blockchain.GetLatestBlock().Index + 1, new DateTime(),
        //             JsonConvert.SerializeObject(dataBlock), blockchain.GetLatestBlock().Hash));
        //         user.HashCode = blockchain.GetLatestBlock().Hash;
        //         BlockchainFileManager.SaveBlockchain(blockchain, "..\\Diplom.Blockchain\\Source\\blockchain.json");
        //         await userRepository.Update(user);
        //     }
        //
        //     await _bufferRepository.Delete(buffer);
        }
    }
    
    public async void AddToBlockchain(Blockchain blockchain)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var _bufferRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Buffer>>();
            
            var buffers = _bufferRepository.GetAll();
        
            if (!buffers.Any())
                return;
            
            foreach (var buffer in buffers)
            {
                using(var scope2 = _serviceScopeFactory.CreateScope())
                {
                    var _userRepository = scope2.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
                    var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == buffer.Login);
                    // CryptographyHelper.Encrypt(buffer.Data, user.PublickKey, out var encryptedData, out var encryptedKey, out var encryptedIv);
                    // var dataBlock = new DataBlock(buffer.Login, buffer.Addressee, buffer.OperationName, encryptedKey, encryptedIv, encryptedData);
                    // blockchain.AddBlock(new Block(blockchain.GetLatestBlock().Index + 1, new DateTime(),
                    //     JsonConvert.SerializeObject(dataBlock), blockchain.GetLatestBlock().Hash));
                    user.HashCode = blockchain.GetLatestBlock().Hash;
                    // BlockchainFileManager.SaveBlockchain(blockchain, "..\\Diplom.Blockchain\\Source\\blockchain.json");
                    using (var scope4 = _serviceScopeFactory.CreateScope())
                    {
                        var _userRepository2 = scope4.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
                        await _userRepository2.Update(user);
                    }
                }
                using (var scope3 = _serviceScopeFactory.CreateScope())
                {
                    var _bufferRepository2 = scope3.ServiceProvider.GetRequiredService<IBaseRepository<Buffer>>();
                    await _bufferRepository2.Delete(buffer);
                }
                
            }
        }

    }
}

