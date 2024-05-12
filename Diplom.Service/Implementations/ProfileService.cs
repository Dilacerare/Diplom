using System.Security.Cryptography;
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
    private readonly IBaseRepository<AccessPermission> _accessRepository;

    public ProfileService(IBaseRepository<User> userRepository,
        ILogger<ProfileService> logger, IBaseRepository<Buffer> bufferRepository, 
        IBaseRepository<AccessPermission> accessRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _bufferRepository = bufferRepository;
        _accessRepository = accessRepository;
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
    
    public async Task<BaseResponse<ProfileViewModel>> AddMedReport(MedicalReportViewModel model)
    {
        try
        {
            Blockchain blockchain = BlockchainFileManager.LoadBlockchain("..\\Diplom.Blockchain\\Source\\blockchain.json");
            
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == model.Login);
            var dataBlock = CryptographyHelper.DataFromBlock(blockchain, user.HashCode);
            
            var data = JsonConvert.DeserializeObject<MedicalRecord>(CryptographyHelper.Decrypt(dataBlock.EncryptedData, user.PrivateKey, dataBlock.EncryptedKey,
                dataBlock.EncryptedIv));
            
            var medicalReport = new MedicalReport()
            {
                Conclusion = model.Conclusion,
                Description = model.Description,
                Recommendation = model.Recommendation
            };
            
            data.MedicalReports ??= new List<MedicalReport>();
            

            data.MedicalReports.Add(medicalReport);
            
            var profile = new ProfileViewModel()
            {
                Login = model.Login,
                PatientName = data.PatientName,
                Age = data.Age,
                Allergies = data.Allergies,
                BloodType = data.BloodType,
                MedicalReports = data.MedicalReports
            };
            
            string textData = JsonConvert.SerializeObject(data);

            var buffer = new Buffer()
            {
                Addressee = model.Login,
                Data = textData,
                Login = model.Initiator,
                OperationName = "Добавление мед записи"
            };

            await _bufferRepository.Create(buffer);
            
            return new BaseResponse<ProfileViewModel>()
            {
                Data = profile,
                Description = "Данные добавлены",
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

    public async Task<BaseResponse<PatientRegisterViewModel>> AddNewPatient(PatientRegisterViewModel model)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == model.Login);
            if (user != null)
            {
                return new BaseResponse<PatientRegisterViewModel>()
                {
                    Description = "Пользователь с таким логином уже есть",
                    StatusCode = StatusCode.UserAlreadyExists
                };
            }
            
            string publicKey;
            string privateKey;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                // Получение публичного и приватного ключей в виде строк
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }
            
            user = new User()
            {
                Login = model.Login,
                Role = Role.Patient,
                Password = HashPasswordHelper.HashPassword(model.Password),
                HashCode = "", 
                PublickKey = publicKey,
                PrivateKey = privateKey
            };
            
            await _userRepository.Create(user);

            var medRecord = new MedicalRecord(
                user.Id, 
                model.PatientName, 
                model.Age, 
                model.BloodType, 
                model.Allergies, 
                null);

            var buffer = new Buffer()
            {
                Addressee = model.Login,
                Data = JsonConvert.SerializeObject(medRecord),
                Login = model.Initiator,
                OperationName = "Регистрация пациента"

            };
            
            await _bufferRepository.Create(buffer);
            
            var access = new AccessPermission()
            {
                Addressee = model.Login,
                Initiator = model.Initiator,
                Date = DateTime.Now,
                Status = AccessStatus.Allowed
            };

            await _accessRepository.Create(access);
            
            return new BaseResponse<PatientRegisterViewModel>()
            {
                Data = model,
                Description = "Пользователь добавлен",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ProfileService.AddNewPatient] error: {ex.Message}");
            return new BaseResponse<PatientRegisterViewModel>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
    
    public async Task<BaseResponse<ProfileAccessViewModel>> GetProfileAccess(string initiator, string addressee)
    {
        try
        {
            AccessPermission? access = await _accessRepository.GetAll().FirstOrDefaultAsync(x => 
                x.Initiator == initiator && 
                x.Addressee == addressee && 
                x.Status == AccessStatus.Allowed
            );
            
            Blockchain blockchain = BlockchainFileManager.LoadBlockchain("..\\Diplom.Blockchain\\Source\\blockchain.json");
            
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == addressee);
            var dataBlock = CryptographyHelper.DataFromBlock(blockchain, user.HashCode);

            var data = JsonConvert.DeserializeObject<MedicalRecord>(CryptographyHelper.Decrypt(dataBlock.EncryptedData, user.PrivateKey, dataBlock.EncryptedKey,
                dataBlock.EncryptedIv));

            if (access != null && access.Date.AddHours(24) >= DateTime.Now)
            {
                var profile = new ProfileAccessViewModel()
                {
                    Access = true,
                    Login = user.Login,
                    PatientName = data.PatientName,
                    Age = data.Age,
                    BloodType = data.BloodType,
                    Allergies = data.Allergies,
                    MedicalReports = data.MedicalReports
                };

                return new BaseResponse<ProfileAccessViewModel>()
                {
                    Data = profile,
                    StatusCode = StatusCode.OK
                };
            }
            else
            {
                if (access != null && access.Date.AddHours(24) <= DateTime.Now)
                {
                    access.Status = AccessStatus.Ended;
                    _accessRepository.Update(access);
                }
                
                var profile = new ProfileAccessViewModel()
                {
                    Access = false,
                    Login = user.Login,
                    PatientName = data.PatientName
                };

                return new BaseResponse<ProfileAccessViewModel>()
                {
                    Data = profile,
                    StatusCode = StatusCode.OK
                };
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ProfileService.GetProfileAccess] error: {ex.Message}");
            return new BaseResponse<ProfileAccessViewModel>()
            {
                StatusCode = StatusCode.InternalServerError,
                Description = $"Внутренняя ошибка: {ex.Message}"
            };
        }
    }
    
}