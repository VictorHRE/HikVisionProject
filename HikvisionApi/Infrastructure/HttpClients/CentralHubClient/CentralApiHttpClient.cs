using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.HttpClients.CentralHubClient;

public class CentralApiHttpClient : ICentralApiHttpClient
{
    private readonly HttpClient _httpClient;

    private string IdStore { get; init; }
    private string Username { get; }
    private string Password { get; }

    private string Token { get; set; }


    public CentralApiHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        IdStore = configuration.GetValue<string>("CentralHubCfg:IdStore")
                  ?? throw new Exception("IdStore Not Found in configuration");
        Username = configuration.GetValue<string>("CentralHubCfg:Username")
                   ?? throw new Exception("Username Not Found in configuration");
        Password = configuration.GetValue<string>("CentralHubCfg:Password")
                   ?? throw new Exception("Password Not Found in configuration");

        var centralApiUrl = configuration.GetValue<string>("CentralHubCfg:ApiUrl")
                            ?? throw new Exception("CentralApiUrl Not Found in configuration");

        centralApiUrl = centralApiUrl.EndsWith($"/") ? centralApiUrl : $"{centralApiUrl}/";

        _httpClient.BaseAddress = new Uri(centralApiUrl);

        Token = string.Empty;
    }

    private void AddHeaders()
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: Token);

        // (Opcional) Aceptar JSON en la respuesta
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
    }

    public string GetIdStore() => IdStore;

    public async Task<HttpResponseMessage> SendPostAsync(string url, string json)
    {
        await GetToken();
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync(GetUrl(url), content);
    }

    /// <summary>
    /// Metodo para obtener el token de la api central
    /// </summary>
    /// <exception cref="HttpRequestException"></exception>
    private async Task GetToken()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        var form = new
        {
            id = 0,
            name = Username,
            email = Username,
            password = Password
        };

        var content = new StringContent(
            content: JsonSerializer.Serialize(form),
            encoding: Encoding.UTF8,
            mediaType: "application/json");

        var request = await _httpClient.PostAsync(
            content: content,
            requestUri: GetUrl("authentication/gettoken")
        );

        var responseBody = await request.Content.ReadAsStringAsync();

        if (!request.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"The Server returned an error: {responseBody}");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<ResponseCentralHubRequest>(
            responseBody,
            options
        );

        Token = result!.Token;

        AddHeaders();
    }

    private static string GetUrl(string url) => url.StartsWith($"/") ? url[..^1] : url;
}