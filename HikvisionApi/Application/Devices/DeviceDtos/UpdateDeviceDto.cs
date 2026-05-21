namespace Application.Devices.DeviceDtos;
public record UpdateDeviceDto(
		int Id,
		string Name,
		string IpAddress,
		string MacAddress,
		string Username,
		string Password,
		int Port,
		string Status
	);