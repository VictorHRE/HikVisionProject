using Shared.Domain.Bus;

namespace Application.Events.CreateEvent;

public class CreateEventCommand : Command
{
	public string Data { get; set; } = string.Empty;

	public int SubeEventType { get; set; }

	public string EmployeeId { get; set; } = string.Empty;
}