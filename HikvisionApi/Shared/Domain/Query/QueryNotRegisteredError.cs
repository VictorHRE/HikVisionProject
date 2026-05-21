namespace Shared.Domain.Query;

public class QueryNotRegisteredError(Query query)
    : Exception($"The query {query} has not a query handler associated")
{
}