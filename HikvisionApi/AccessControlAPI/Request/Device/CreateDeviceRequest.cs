namespace AMPMAccessControlAPI.Dtos.Device;

public record CreateDeviceRequest(
		string Name,
		string IpAddress,
		string DeviceMacAddress,
		string Username,
		string Password,
		int Port,
		string Status
	);