using Shared.Domain.Bus;

namespace Application.Devices.SetHttpListener;

public class SetHttpListenerCommand : Command
{
	public int Port { get; set; }

	public string IpAddress { get; set; } = string.Empty;

	public string Protocol { get; set; } = string.Empty;

	public string Url { get; set; } = string.Empty;
}
