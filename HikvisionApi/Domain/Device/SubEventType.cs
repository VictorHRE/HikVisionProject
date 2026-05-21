namespace Domain.Device;

public enum SubEventType
{
	//for more details about SubTypeEvents see Device Network SDK (Person-Based Access Control) Developer Guide
	//2.9.1 Access Control Event Types
	
	WITHOUT_PERMISSION = 6,
	INVALID_PERIDIOD_DURARTION = 7,
	PERMISSION_EXPIRED = 8,
	FINGER_PRINT_ACCESS_GRANTED = 38,
	FINGER_PRINT_ACCESS_DENIED = 39,
	DOOR_OPEN = 21,
	DOOR_CLOSE = 22,
	UNKNOWN = 0
}
