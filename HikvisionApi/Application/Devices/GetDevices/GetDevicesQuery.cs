using Shared.Domain.Query;

namespace Application.Devices.GetDevices;

public class GetDevicesQuery : Query
{
	public string? Name { get; set; }

	public string? IpAddress { get; set; }
}
