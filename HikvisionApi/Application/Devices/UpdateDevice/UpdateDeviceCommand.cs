using Shared.Domain.Bus;

namespace Application.Devices.UpdateDevice;

public class UpdateDeviceCommand : Command
{
	public int Id { get; set; }

	public string DeviceName { get; set; }

	public string IpAddress { get; set; }

	public string DeviceMacAddress { get; set; }

	public string Username { get; set; }

	public string Password { get; set; }

	public int Port { get; set; }

	public string Status { get; set; }

	public UpdateDeviceCommand(
		int id, 
		string deviceName, 
		string ipAddress, 
		string deviceMacAddress,
		string username, 
		string password, 
		int port, 
		string status)
	{
		Id = id;
		DeviceName = deviceName;
		IpAddress = ipAddress;
		DeviceMacAddress = deviceMacAddress;
		Username = username;
		Password = password;
		Port = port;
		Status = status;
	}
}
