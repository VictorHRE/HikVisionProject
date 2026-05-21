namespace Application.Devices.DeviceDtos;

public record DeviceDto(
		int Id,
		string Name,
		string IpAddress,
		string MacAddress,
		string Username,
		string Password,
		int Port,
		string Status,
		DateTime CreatedAt,
		DateTime? UpdatedAt
	);