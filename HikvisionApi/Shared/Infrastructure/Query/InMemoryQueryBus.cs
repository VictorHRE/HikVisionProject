using System.Collections;
using System.Collections.Concurrent;
using Shared.Domain.Query;

namespace Shared.Infrastructure.Query;

public class InMemoryQueryBus(IServiceProvider serviceProvider) : IQueryBus
{
    private static readonly ConcurrentDictionary<Type, object> QueryHandlers = new();

    public async Task<TResponse> AskAsync<TResponse>(Domain.Query.Query query)
    {
        var handler = GetHandlerWrapper<TResponse>(query);

        return handler is null
            ? throw new QueryNotRegisteredError(query)
            : await handler.Handle(query, serviceProvider);
    }

    private QueryHandlerWrapper<TResponse> GetHandlerWrapper<TResponse>(Domain.Query.Query query)
    {
        Type[] typeargs = { query.GetType(), typeof(TResponse) };

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeargs);
        var wrapperType = typeof(QueryHandlerWrapper<,>).MakeGenericType(typeargs);

        var handlers = (IEnumerable)serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType))!;

        var wrappedHandlers = (QueryHandlerWrapper<TResponse>)QueryHandlers.GetOrAdd(query.GetType(),
            handlers.Cast<object>()
                .Select(handler => (QueryHandlerWrapper<TResponse>)Activator.CreateInstance(wrapperType))
                .FirstOrDefault());

        return wrappedHandlers;
    }
}
