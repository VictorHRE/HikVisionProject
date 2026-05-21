using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AMPM_CentralHubAPI.DI;

public static class Infrastructure
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{


		services.AddDbContext<AmPmCentralHubContext>(options =>
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection");

			if (string.IsNullOrEmpty(connectionString))
			{
				throw new Exception("Connection string 'DefaultConnection' not found.");
			}

			options.UseSqlServer(connectionString, sqlOptions =>
			{
				sqlOptions.EnableRetryOnFailure(
					maxRetryCount: 5,
					maxRetryDelay: TimeSpan.FromSeconds(30),
					errorNumbersToAdd: null);
			});
		});

		return services;
	}
}
