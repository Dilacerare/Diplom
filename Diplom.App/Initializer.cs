using Diplom.DAL.Interfaces;
using Diplom.DAL.Repositories;
using Diplom.Domain.Entity;
using Diplom.Service.Implementations;
using Diplom.Service.Interfaces;

namespace Diplom.App;

public static class Initializer
{
    public static void InitializeRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<User>, UserRepository>();
    }

    public static void InitializeServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
    }
}