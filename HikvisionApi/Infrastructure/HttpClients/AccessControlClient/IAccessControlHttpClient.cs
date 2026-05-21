namespace Infrastructure.HttpClients.AccessControlClient;

public interface IAccessControlHttpClient
{
    /// <summary>
    /// send request for get content GET
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetAsync(string url);

    /// <summary>
    /// send request for delete something in the api POST
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteAsync(string url);

    /// <summary>
    /// send request for create or get content from api DELETE
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> PostAsync(string url, string body);

    /// <summary>
    /// send a request for update PUT
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> PutAsync(string url, string body);

    /// <summary>
    /// change the base url of the client
    /// </summary>
    /// <param name="baseUrl"></param>"
    bool changeBaseUrl(string baseUrl);
}