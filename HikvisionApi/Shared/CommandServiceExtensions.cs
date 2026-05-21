using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Bus;

namespace Shared;

public static class CommandServiceExtensions
{
    public static IServiceCollection AddCommandServices(this IServiceCollection services, Assembly? assembly)
    {
        var handlerInterfaceType = typeof(ICommandHandler<,>);

        if (assembly == null) return services;

        var commandHandlerTypes = assembly.ExportedTypes
            .Select(t => t.GetTypeInfo())
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(type => new
            {
                Implementation = type,
                Interfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
            });

        foreach (var handler in commandHandlerTypes)
        {
            foreach (var @interface in handler.Interfaces)
            {
                services.AddScoped(@interface, handler.Implementation);
            }
        }

        return services;
    }
}