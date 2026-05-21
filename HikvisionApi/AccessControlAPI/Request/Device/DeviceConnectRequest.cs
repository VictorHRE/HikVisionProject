namespace AMPMAccesControlAPI.Request.Device;

public record DeviceConnectRequest(string Protocol, string IP, int Port, string Username, string Password);