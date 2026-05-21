namespace Domain.Device;

public class Device
{

	public DeviceId Id { get; set; }

	public string Name { get; set; }

	public string IpAddress { get; set; }

	public DeviceMacAddress DeviceMacAddress { get; set; }

	public string Username { get; set; }

	public string Password { get; set; }

	public int Port { get; set; }

	public DeviceStatus Status { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public Device(DeviceId id,
		string name,
		string ipAddress,
		DeviceMacAddress deviceMacAddress,
		string username,
		string password,
		int port,
		DeviceStatus status,
		DateTime createdAt,
		DateTime updatedAt)
	{
		Id = id;
		Name = name;
		IpAddress = ipAddress;
		DeviceMacAddress = deviceMacAddress;
		Username = username;
		Password = password;
		Port = port;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public Device()
	{
		Id = new DeviceId(0);
		Name = string.Empty;
		IpAddress = string.Empty;
		DeviceMacAddress = new DeviceMacAddress(string.Empty);
		Username = string.Empty;
		Password = string.Empty;
		Port = 80;
		Status = DeviceStatus.UNKNOWN;
		CreatedAt = DateTime.UtcNow;
		UpdatedAt = DateTime.UtcNow;
	}

	public static Device Create(
		string name,
		string ipAddress,
		DeviceMacAddress deviceMacAddress,
		string username,
		string password,
		DeviceStatus status,
		int port)
	{
		return new Device
		{
			Name = name,
			IpAddress = ipAddress,
			DeviceMacAddress = deviceMacAddress,
			Username = username,
			Password = password,
			Status = status,
			Port = port
		};
	}

	public void Update(Device device)
	{
		Name = device.Name;
		IpAddress = device.IpAddress;
		Username = device.Username;
		Password = device.Password;
		Status = device.Status;
		Port = device.Port == 0 ? 80 : device.Port;
		UpdatedAt = DateTime.UtcNow;
	}
}

