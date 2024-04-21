using Diplom.BlockchainApp.Entity;
using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;
using Diplom.Domain.Enum;
using Diplom.Domain.Extensions;
using Diplom.Domain.Helpers;
using Diplom.Domain.Response;
using Diplom.Domain.ViewModels;
using Diplom.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.Service.Implementations;

public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Buffer> _bufferRepository;

    public ProfileService(IBaseRepository<User> userRepository,
        ILogger<ProfileService> logger, IBaseRepository<Buffer> bufferRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _bufferRepository = bufferRepository;
    }
    
    public async Task<BaseResponse<ProfileViewModel>> GetProfile(string login)
    {
        try
        {
            Blockchain blockchain = BlockchainFileManager.LoadBlockchain("..\\Diplom.Blockchain\\Source\\blockchain.json");
            
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == login);
            var dataBlock = CryptographyHelper.DataFromBlock(blockchain, user.HashCode);

            var data = JsonConvert.DeserializeObject<MedicalRecord>(CryptographyHelper.Decrypt(dataBlock.EncryptedData, user.PrivateKey, dataBlock.EncryptedKey,
                dataBlock.EncryptedIv));
            
            var profile = new ProfileViewModel()
            {
                Login = user.Login,
                PatientName = data.PatientName,
                Age = data.Age,
                BloodType = data.BloodType,
                Allergies = data.Allergies,
                MedicalReports = data.MedicalReports
            };

            return new BaseResponse<ProfileViewModel>()
            {
                Data = profile,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ProfileService.GetProfile] error: {ex.Message}");
            return new BaseResponse<ProfileViewModel>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }

    public async Task<BaseResponse<ProfileViewModel>> Save(ProfileViewModel model)
    {
        try
        {
            Blockchain blockchain = BlockchainFileManager.LoadBlockchain("..\\Diplom.Blockchain\\Source\\blockchain.json");
            
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == model.Login);
            var dataBlock = CryptographyHelper.DataFromBlock(blockchain, user.HashCode);
            
            var data = JsonConvert.DeserializeObject<MedicalRecord>(CryptographyHelper.Decrypt(dataBlock.EncryptedData, user.PrivateKey, dataBlock.EncryptedKey,
                dataBlock.EncryptedIv));

            var profile = new ProfileViewModel()
            {
                Login = model.Login,
                PatientName = model.PatientName,
                Age = model.Age,
                Allergies = model.Allergies,
                BloodType = model.BloodType,
                MedicalReports = data.MedicalReports
            };

            data.PatientName = model.PatientName;
            data.Age = model.Age;
            data.Allergies = model.Allergies;
            data.BloodType = model.BloodType;
            data.MedicalReports = data.MedicalReports;
            
            string textData = JsonConvert.SerializeObject(data);

            var buffer = new Buffer()
            {
                Addressee = user.Login,
                Data = textData,
                Login = user.Login,
                OperationName = "Редактирование профиля"
            };

            await _bufferRepository.Create(buffer);
            
            // string encryptedData;
            // string encryptedKey;
            // string encryptedIv;
            //
            // CryptographyHelper.Encrypt(textData, user.PublickKey, out encryptedData, out encryptedKey, out encryptedIv);
            // dataBlock = new DataBlock(user.Login, user.Login, "Редактирование профиля", encryptedKey, encryptedIv, encryptedData);
            // blockchain.AddBlock(new Block(blockchain.GetLatestBlock().Index + 1, new DateTime(),
            //     JsonConvert.SerializeObject(dataBlock), blockchain.GetLatestBlock().Hash));
            // user.HashCode = blockchain.GetLatestBlock().Hash;
            // BlockchainFileManager.SaveBlockchain(blockchain, "..\\Diplom.Blockchain\\Source\\blockchain.json");
            // await _userRepository.Update(user);
            
            return new BaseResponse<ProfileViewModel>()
            {
                Data = profile,
                Description = "Данные обновлены",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ProfileService.Save] error: {ex.Message}");
            return new BaseResponse<ProfileViewModel>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
}