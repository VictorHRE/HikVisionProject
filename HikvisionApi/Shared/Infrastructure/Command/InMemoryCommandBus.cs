using Shared.Domain.Bus;
using Shared.Domain.Query;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Shared.Infrastructure.Command;

public class InMemoryCommandBus(IServiceProvider serviceProvider) : ICommandBus
{
	private static readonly ConcurrentDictionary<Type, object> _handlers = new();


	public async Task<TResponse> DispatchAsync<TResponse>(Domain.Bus.Command command)
    {
        
        var wrappedHandler = GetWrappedHandlers<TResponse>(command) ?? throw new CommandNotRegisteredError(command);
		
		return await wrappedHandler.Handle(command, serviceProvider);


	/*	var commandHandlerWrappers = wrappedHandlers as CommandHandlerWrapper[] ?? [.. wrappedHandlers];
        
        if (commandHandlerWrappers.Length == 0) throw new CommandNotRegisteredError(command);

        foreach (var wrappedHandler in commandHandlerWrappers) await wrappedHandler.Handle(command, serviceProvider);*/
    }

    private CommandHandlerWrapper<TResponse> GetWrappedHandlers<TResponse>(Domain.Bus.Command command)
    {
		Type[] typeargs = { command.GetType(), typeof(TResponse) };

		var handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeargs);
		var wrapperType = typeof(CommandHandlerWrapper<,>).MakeGenericType(typeargs);

		var handlers = (IEnumerable)serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType))!;

		var wrappedHandlers = (CommandHandlerWrapper<TResponse>)_handlers.GetOrAdd(command.GetType(),
			handlers.Cast<object>()
				.Select(handler => (CommandHandlerWrapper<TResponse>)Activator.CreateInstance(wrapperType))
				.FirstOrDefault());

		return wrappedHandlers;
    }
}