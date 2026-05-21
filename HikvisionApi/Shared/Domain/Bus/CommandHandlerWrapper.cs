namespace Shared.Domain.Bus;

internal abstract class CommandHandlerWrapper<TResponse>
{
    public abstract Task<TResponse> Handle(Command command, IServiceProvider serviceProvider);
}

internal class CommandHandlerWrapper<TCommand, TResponse> : CommandHandlerWrapper<TResponse>
    where TCommand : Command
{
    public override async Task<TResponse> Handle(Command command, IServiceProvider serviceProvider)
    {
        var handler = (ICommandHandler<TCommand, TResponse>)serviceProvider.GetService(typeof(ICommandHandler<TCommand, TResponse>))!;

        return await handler.Handle((TCommand)command);
    }
}