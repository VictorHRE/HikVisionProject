namespace Infrastructure.HttpClients.CentralHubClient;

public interface ICentralApiHttpClient
{
    string GetIdStore();
    Task<HttpResponseMessage> SendPostAsync(string url, string json);
}