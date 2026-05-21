
namespace Application.Devices.DeviceDtos;

public class HttpListenerDto
{
	public int Id { get; set; }

	public string Url { get; set; } = string.Empty;

	public int Port { get; set; }

	public string Protocol { get; set; } = string.Empty;

	public string IpAddress { get; set; } = string.Empty;
}
