namespace AMPMAccesControlAPI.Request.Event;

public record AccessControllerEventRequest(
	string? DeviceName,
	int? MajorEventType,
	int? SubEventType,
	int? VerfyNo,
	string? Name,
	string? EmployeeNoString,
	int? SerialNo,
	string? CurrentVerifyMode,
	int? FrontSerialNo,
	string? AttendanceStatus);
