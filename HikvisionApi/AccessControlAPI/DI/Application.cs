using Domain.Attendance;
using Domain.Device;
using Domain.Employee;
using Infrastructure.HttpClients.CentralHubClient;
using Infrastructure.HttpClients.DigestClient;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using Shared;
using Shared.Domain.Bus;
using Shared.Domain.Query;
using Shared.Helpers;
using Shared.Infrastructure.Command;
using Shared.Infrastructure.Query;

namespace AMPMAccesControlAPI.DI;

public static class Application
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{

		services.AddCommandServices(AssemblyHelper.GetInstance(Assemblies.Infrastructure));
		services.AddQueryServices(AssemblyHelper.GetInstance(Assemblies.Infrastructure));

		services.AddCommandServices(AssemblyHelper.GetInstance(Assemblies.Domain));
		services.AddQueryServices(AssemblyHelper.GetInstance(Assemblies.Domain));

		services.AddCommandServices(AssemblyHelper.GetInstance(Assemblies.Application));
		services.AddQueryServices(AssemblyHelper.GetInstance(Assemblies.Application));

		services.AddScoped<IQueryBus, InMemoryQueryBus>();
		services.AddScoped<ICommandBus, InMemoryCommandBus>();

		services.AddScoped<IDeviceRepository, DeviceRepository>();
		services.AddScoped<IEmployeeRepository, EmployeeRepository>();

		services.AddScoped<IUnitOfWork, UnitOfWork>();

		services.AddScoped<DigestAuthClient, DigestAuthClient>();
		services.AddScoped<ICentralApiHttpClient, CentralApiHttpClient>();

		services.AddScoped<IAttendanceRepository, AttendanceRepository>();


		return services;
	}
}
