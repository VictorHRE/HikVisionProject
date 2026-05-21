namespace Domain.Host;

public class HttpListener
{
	
	// Id of the listener in the device
	public int Id { get; set; }

	// Url of the listener, referer to ip of this project
	public string Url { get; set; }

	// Port of the listener, port of this project where the server is running
	public int Port { get; set; }

	// Protocol of the listener, http or https
	public string Protocol { get; set; }

	// IpAddress of the listener, defined in the controller /Event/
	public string IpAddress { get; set; }


	public HttpListener()
	{
		Url = string.Empty;
		Protocol = string.Empty;
		IpAddress = string.Empty;
	}

	public HttpListener(int id, string url, int port, string protocol, string ipAddress)
	{
		Id = id;
		Url = url;
		Port = port;
		Protocol = protocol;
		IpAddress = ipAddress;
	}
}