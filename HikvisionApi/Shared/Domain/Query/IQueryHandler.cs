namespace Shared.Domain.Query;

public interface IQueryHandler<TQuery, TResponse> where TQuery : Query
{
    Task<TResponse> Handle(TQuery query);
}