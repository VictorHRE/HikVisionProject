namespace Domain.Device;

public class AccessControllerEvent
{
	public string? DeviceName { get; set; }

	public int? MajorEventType { get; set; }

	public int? SubEventType { get; set; }

	public int? VerfyNo { get; set; }

	public string? Name { get; set; }

	public string? EmployeeNoString { get; set; }

	public int? SerialNo { get; set; }

	public string? CurrentVerifyMode { get; set; }

	public int? FrontSerialNo { get; set; }

	public string? AttendanceStatus { get; set; }
}