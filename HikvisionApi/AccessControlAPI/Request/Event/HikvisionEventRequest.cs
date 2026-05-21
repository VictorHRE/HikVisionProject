namespace AMPMAccesControlAPI.Request.Event;

public record HikvisionEventRequest(
	string? IpAddress,
	int? ChannelId,
	string? DateTime,
	string? EventType,
	string? EventState,
	string? EventDescription,
	string? DeviceId,
	string? Event_Log,
	AccessControllerEventRequest AccessControllerEvent);
