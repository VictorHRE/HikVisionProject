using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.HttpClients.AccessControlClient;

public class AccessControlHttpClient : IAccessControlHttpClient
{
    private readonly HttpClient _httpClient;

    private string ApiUrl { get; set; }


    public AccessControlHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        ApiUrl = configuration.GetValue<string>("AccessControlCfg:ApiUrl")
                 ?? throw new Exception("ApiUrl is missing in appsettings.json");

        ApiUrl = ApiUrl.EndsWith($"/") ? ApiUrl : $"{ApiUrl}/";

        _httpClient.BaseAddress = new Uri(ApiUrl);

        AddHeaders();
    }

    private void AddHeaders()
    {
        // (Opcional) Aceptar JSON en la respuesta
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
        => await SendRequest(
            HttpMethod.Get,
            url,
            string.Empty
        );

    public async Task<HttpResponseMessage> DeleteAsync(string url)
        => await SendRequest(
            HttpMethod.Delete,
            url,
            string.Empty
        );

    public async Task<HttpResponseMessage> PostAsync(string url, string body)
        => await SendRequest(
            HttpMethod.Post,
            url,
            body
        );

    public async Task<HttpResponseMessage> PutAsync(string url, string body)
        => await SendRequest(
            HttpMethod.Put,
            url,
            body
        );

    private async Task<HttpResponseMessage> SendRequest(
        HttpMethod method,
        string url,
        string? body,
        string? contentType = "application/json")
    {
        // 1. Primera petición para obtener el desafío (challenge)
        var requestUri = GetUrl(url);


        // 4. Reintentar la petición con el encabezado Authorization
        var request = new HttpRequestMessage(method, url);

        var methosds = new[] { HttpMethod.Post, HttpMethod.Put };

        if (body is not null) // Si es POST, se agrega el Content-Type al encabezado de la petición
        {
            request = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(body, Encoding.UTF8, contentType!)
            };
        }

        return await _httpClient.SendAsync(request);
    }

    private static string GetUrl(string url) => url.StartsWith($"/") ? url[..^1] : url;

    public bool changeBaseUrl(string baseUrl)
    {
        try
        {
            ApiUrl = baseUrl.EndsWith($"/") ? baseUrl : $"{baseUrl}/";
            _httpClient.BaseAddress = new Uri(ApiUrl);
            return true;
        }
        catch (Exception )
        {
            return false;
        }
    }
}