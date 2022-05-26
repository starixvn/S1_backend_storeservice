using StoreService.Core.Service.BLL;
using StoreService.Core.Service.DAL;
using StoreService.Core.Service.Library;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<MySqlClient>>();
            
            return services
                .AddSingleton(param =>
                    new MySqlClient(configuration.GetConnectionString("ConnectionString"), logger)
                    {
                        DataBase = DataBaseEnum.Main
                    })
                .AddSingleton(param =>
                    new MySqlClient(configuration.GetConnectionString("ConnectionStringReport"), logger)
                    {
                        DataBase = DataBaseEnum.Report
                    });
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddMemoryCache()
                .AddSingleton<CacheHelper>()
                //.AddTransient<CourierAdapter>()
                .AddSingleton<GiftManagementService>()
                .AddSingleton<GiftManagementBLL>();
        }
}