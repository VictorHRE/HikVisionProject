namespace Application.Devices.DeviceDtos;

public record CreateDeviceDto(
		string Name,
		string IpAddress,
		string DeviceMacAddress,
		string Username,
		string Password,
		int Port,
		string Status
	);