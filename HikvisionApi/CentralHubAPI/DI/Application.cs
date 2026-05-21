using Domain.Device;
using Domain.Employee;
using Infrastructure.HttpClients.AccessControlClient;
using Infrastructure.HttpClients.DigestClient;
using Infrastructure.HttpClients.HumandClient;
using Infrastructure.Repositories;

namespace AMPM_CentralHubAPI.DI;

public static class Application
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{


		//services.AddScoped<IQueryBus, InMemoryQueryBus>();
		//services.AddScoped<ICommandBus, InMemoryCommandBus>();

		//services.AddScoped<IDeviceRepository, DeviceRepository>();
		//services.AddScoped<IEmployeeRepository, EmployeeRepository>();

		//services.AddScoped<IUnitOfWork, UnitOfWork>();

		services.AddScoped<DigestAuthClient, DigestAuthClient>();
		services.AddScoped<IHumanHttpClient, HumanHttpClient>();
		services.AddScoped<HumandEmployeeRepository, HumandEmployeeRepository>();
        services.AddScoped<IAccessControlHttpClient, AccessControlHttpClient>();
        services.AddScoped<AccessControlRepository, AccessControlRepository>();

        return services;
	}
}
