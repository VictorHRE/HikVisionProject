namespace Infrastructure.HttpClients.HumandClient;

public interface IHumanHttpClient
{
    Task<HttpResponseMessage> SendPostRequestAsync(string url, object? data = null);

    Task<HttpResponseMessage> SendPutRequestAsync(string url, object? data = null);

    Task<HttpResponseMessage> SendDeleteRequestAsync(string url);

    Task<HttpResponseMessage> SendGetRequestAsync(string url);
}