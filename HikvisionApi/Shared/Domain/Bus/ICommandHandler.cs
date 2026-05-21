namespace Shared.Domain.Bus;

public interface ICommandHandler<TCommand, TResponse> where TCommand : Command
{
    Task<TResponse> Handle(TCommand command);
}
