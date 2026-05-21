namespace Infrastructure.HttpClients.CentralHubClient;

public class ResponseCentralHubRequest
{
    public bool IsSuccess { get; set; } = false;

    public string Token { get; set; } = string.Empty;
}