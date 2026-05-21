namespace AMPMAccesControlAPI.Request.HttpListener;

public class HttpListenerRequest
{
	public int Port { get; set; }

	public string IpAddress { get; set; } = string.Empty;

	public string Protocol { get; set; } = string.Empty;

	public string Url { get; set; } = string.Empty;
}
