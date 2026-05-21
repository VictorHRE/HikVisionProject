namespace Application.Events.CreateEvent;

public class CreateEventLogRequest
{
    public int Id { get; set; }

    public string Data { get; set; }

    public string EventType { get; set; }

    public DateTime CreatedAt { get; set; }

    public string IdStoreHQ { get; set; }
    public string EmployeeIdentification { get; set; }
}