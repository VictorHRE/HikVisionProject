using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Query;

namespace Shared;

public static class QueryServiceExtension
{
    public static IServiceCollection AddQueryServices(this IServiceCollection services, Assembly? assembly)
    {
        if (assembly == null) return services;

        var classTypes = assembly.ExportedTypes.Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in classTypes)
        {
            var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo());

            foreach (var handlerInterfaceType in interfaces.Where(i =>
                         i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                services.AddScoped(handlerInterfaceType.AsType(), type.AsType());
        }

        return services;
    }
}