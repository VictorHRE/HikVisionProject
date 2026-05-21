namespace AMPMAccessControlAPI.Dtos.Device;

public record DeviceRequest(
		int Id,
		string DeviceName,
		string IpAddress,
		string DeviceMacAddress,
		string Username,
		string Password,
		int Port,
		string Status,
		DateTime CreatedAt,
		DateTime? UpdatedAt
	);