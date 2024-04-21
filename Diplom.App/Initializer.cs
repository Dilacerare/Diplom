using Diplom.DAL.Interfaces;
using Diplom.DAL.Repositories;
using Diplom.Domain.Entity;
using Diplom.Service.Implementations;
using Diplom.Service.Interfaces;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.App;

public static class Initializer
{
    public static void InitializeRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<User>, UserRepository>();
        services.AddScoped<IBaseRepository<Buffer>, BufferRepository>();
    }

    public static void InitializeServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IBufferService, BufferService>();
    }
}