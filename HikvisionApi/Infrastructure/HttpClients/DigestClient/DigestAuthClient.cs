using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.HttpClients.DigestClient;

public class DigestAuthClient
{

	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IConfiguration _configuration;
	private readonly HttpClient _httpClient;

	private readonly string _baseUrl;
	private readonly string _password;
	private readonly string _username;

	public DigestAuthClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{
		_configuration = configuration;
		_httpClientFactory = httpClientFactory;

		// Configuración de la URL base
		_baseUrl = _configuration.GetValue<string>("HikVisionDevice:Host")
		?? throw new Exception("No se pudo obtener la URL base de la configuración.");

		_baseUrl = _baseUrl.EndsWith("/") ? _baseUrl[..^1] : _baseUrl;

		// Configuración de las credenciales
		_username = _configuration.GetValue<string>("HikVisionDevice:Username")
			?? throw new Exception("No se pudo obtener el nombre de usuario de la configuración.");

		_password = _configuration.GetValue<string>("HikVisionDevice:Password")
			?? throw new Exception("No se pudo obtener el nombre de usuario de la configuración.");

		_httpClient = _httpClientFactory.CreateClient();

		_httpClient.BaseAddress = new Uri(_baseUrl);
	}

	
	public async Task<HttpResponseMessage> GetDigestAuthAsync(string url, string? contentType = null)
	{
		var request = await SendRequestWithDigestAuth(
			HttpMethod.Get,
			url,
			string.Empty,
			contentType ?? "application/xml"
		);

		return request;
	}

	public async Task<HttpResponseMessage> PostDigestAuthAsync(string url, string body, string? contentType)
	{

		var request = await SendRequestWithDigestAuth(
			HttpMethod.Post,
			url,
			body,
			contentType ?? "application/xml"
		);

		return request;
	}

	public async Task<HttpResponseMessage> PutDigestAuthAsync(string url, string body, string? contentType)
	{

		var request = await SendRequestWithDigestAuth(
			HttpMethod.Put,
			url,
			body,
			contentType ?? "application/xml"
		);

		return request;
	}

	private async Task<HttpResponseMessage> SendRequestWithDigestAuth(
		HttpMethod method,
		string url,
		string body,
		string contentType = "application/xml")
	{
		// 1. Primera petición para obtener el desafío (challenge)
		var _url = url.StartsWith('/') ? url[1..] : url;

		var initialResponse = await _httpClient.GetAsync(_url);

		if (initialResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
			return initialResponse; // No requiere autenticación o ya está autorizado

		if (!initialResponse.Headers.WwwAuthenticate.Any())
			throw new Exception("No se encontró el encabezado WWW-Authenticate para Digest Auth");

		// 2. Extraer los parámetros del encabezado WWW-Authenticate
		var wwwAuthHeader = initialResponse.Headers.WwwAuthenticate
			.First(h => h.Scheme.Equals("Digest", StringComparison.OrdinalIgnoreCase));

		var parameters = ParseDigestHeader(wwwAuthHeader.Parameter!);

		// 3. Calcular el header Authorization
		var digestHeader = BuildDigestHeader(parameters, method.Method, _url, _username, _password);

		// 4. Reintentar la petición con el encabezado Authorization
		var request = new HttpRequestMessage(method, url);

		var methosds = new[] { HttpMethod.Post, HttpMethod.Put };

		if (body is not null) // Si es POST, se agrega el Content-Type al encabezado de la petición
		{
			request = new HttpRequestMessage(method, url)
			{
				Content = new StringContent(body, Encoding.UTF8, contentType)
			};
		}

		request.Headers.Authorization = new AuthenticationHeaderValue("Digest", digestHeader);

		return await _httpClient.SendAsync(request);
	}

	private static Dictionary<string, string> ParseDigestHeader(string header)
	{
		// Regex para extraer pares key=value (soporta valores entre comillas)
		var dict = new Dictionary<string, string>();
		var regex = new Regex(@"(\w+)=(""([^""]*)""|([^,]*))", RegexOptions.Compiled);

		foreach (Match match in regex.Matches(header))
		{
			var key = match.Groups[1].Value;
			var val = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Value;
			dict[key] = val;
		}

		return dict;
	}

	private static string BuildDigestHeader(
		Dictionary<string, string> parameters,
		string httpMethod,
		string uri,
		string username,
		string password)
	{
		string realm = parameters["realm"];
		string nonce = parameters["nonce"];
		string? qop = parameters.ContainsKey("qop") ? parameters["qop"] : null;
		string? opaque = parameters.ContainsKey("opaque") ? parameters["opaque"] : null;
		string algorithm = parameters.ContainsKey("algorithm") ? parameters["algorithm"] : "MD5";
		string nc = "00000001"; // nonce count
		string cnonce = CreateCnonce();

		// HA1
		string ha1Raw = $"{username}:{realm}:{password}";
		string ha1 = HashMD5(ha1Raw);

		// HA2
		string ha2Raw = $"{httpMethod}:{uri}";
		string ha2 = HashMD5(ha2Raw);

		// Response
		string responseRaw;
		if (!string.IsNullOrEmpty(qop))
		{
			responseRaw = $"{ha1}:{nonce}:{nc}:{cnonce}:{qop}:{ha2}";
		}
		else
		{
			responseRaw = $"{ha1}:{nonce}:{ha2}";
		}
		string response = HashMD5(responseRaw);

		// Construir header digest
		var headerBuilder = new StringBuilder();
		headerBuilder.Append($"username=\"{username}\", ");
		headerBuilder.Append($"realm=\"{realm}\", ");
		headerBuilder.Append($"nonce=\"{nonce}\", ");
		headerBuilder.Append($"uri=\"{uri}\", ");
		headerBuilder.Append($"response=\"{response}\", ");

		if (!string.IsNullOrEmpty(opaque))
			headerBuilder.Append($"opaque=\"{opaque}\", ");
		if (!string.IsNullOrEmpty(qop))
		{
			headerBuilder.Append($"qop={qop}, ");
			headerBuilder.Append($"nc={nc}, ");
			headerBuilder.Append($"cnonce=\"{cnonce}\", ");
		}
		headerBuilder.Append($"algorithm={algorithm}");

		return headerBuilder.ToString();
	}

	private static string CreateCnonce()
	{
		var random = new byte[16];
		RandomNumberGenerator.Fill(random);
		return Convert.ToBase64String(random);
	}

	private static string HashMD5(string input)
	{
		/*
			using var md5 = MD5.Create();
			var inputBytes = Encoding.UTF8.GetBytes(input);
			var hashBytes = md5.ComputeHash(inputBytes);
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
		 */

		var inputBytes = Encoding.UTF8.GetBytes(input);
		var hashBytes = MD5.HashData(inputBytes);

		return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
	}
}