namespace Shared.Domain.Query;

internal abstract class QueryHandlerWrapper<TResponse>
{
    public abstract Task<TResponse> Handle(Query query, IServiceProvider serviceProvider);
}

internal class QueryHandlerWrapper<TQuery, TResponse> : QueryHandlerWrapper<TResponse> where TQuery : Query
{
    public override Task<TResponse> Handle(Query query, IServiceProvider serviceProvider)
    {
        var handler =
            (IQueryHandler<TQuery, TResponse>)serviceProvider.GetService(typeof(IQueryHandler<TQuery, TResponse>))!;

        return handler.Handle((TQuery)query);
    }
}
