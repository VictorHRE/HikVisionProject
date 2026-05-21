namespace Shared.Domain.Query;

public interface IQueryBus
{
    Task<TResponse> AskAsync<TResponse>(Query query);
}