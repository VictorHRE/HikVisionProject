namespace Shared.Domain.Bus;

public interface ICommandBus
{
    Task<TResponse> DispatchAsync<TResponse>(Command command);
}