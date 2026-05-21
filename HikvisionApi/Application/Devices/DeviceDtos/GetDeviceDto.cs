namespace Application.Devices.DeviceDtos;

public record GetDeviceDto(
	int Id,
	string Name,
	string IpAddress,
	string DeviceMacAddress,
	string Username,
	string Password,
	int Port,
	string Status,
	DateTime CreatedAt,
	DateTime? UpdatedAt = null);