using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.HttpClients.HumandClient;

public class HumanHttpClient : IHumanHttpClient
{
    private readonly HttpClient _httpClient;

    private readonly string _apiKey;

    public HumanHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        var baseUrl = configuration.GetValue<string>("Humand:ApiUrl")
                      ?? throw new Exception("No  ApiUrl configured");

        baseUrl = baseUrl.EndsWith($"/") ? baseUrl : $"{baseUrl}/";

        // Configuración de las credenciales
        _apiKey = configuration.GetValue<string>("Humand:ApiKey")
                  ?? throw new Exception("No apikey found in configuration.");

        _httpClient = httpClientFactory.CreateClient();

        _httpClient.BaseAddress = new Uri(baseUrl);

        AddHeaders();
    }

    public async Task<HttpResponseMessage> SendPostRequestAsync(string url, object? data = null)
    {
        if (data is null) return await _httpClient.PostAsync(url, null);

        var jsonBody = data;
        var content = new StringContent((string)jsonBody, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync(GetUrl(url), content);
    }

    public async Task<HttpResponseMessage> SendPutRequestAsync(string url, object? data = null)
    {
        if (data is not null) return await _httpClient.PostAsync(url, null);

        var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        return await _httpClient.PutAsync(requestUri: GetUrl(url), content: content);
    }

    public async Task<HttpResponseMessage> SendDeleteRequestAsync(string url)
        => await _httpClient.DeleteAsync(requestUri: GetUrl(url));

    public async Task<HttpResponseMessage> SendGetRequestAsync(string url)
        => await _httpClient.GetAsync(GetUrl(url));

    private HttpClient AddHeaders()
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _apiKey);

        // (Opcional) Aceptar JSON en la respuesta
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return _httpClient;
    }

    private static string GetUrl(string url)
    {
        url = url.StartsWith("/") ? url.Substring(0, url.Length - 1) : url;

        return url;
    }
}