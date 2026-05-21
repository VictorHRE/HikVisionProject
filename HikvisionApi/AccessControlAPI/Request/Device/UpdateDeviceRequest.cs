namespace AMPMAccessControlAPI.Dtos.Device;

public record UpdateDeviceRequest(
		string Name,
		string IpAddress,
		string MacAddress,
		string Username,
		string Password,
		int Port,
		string Status
	);