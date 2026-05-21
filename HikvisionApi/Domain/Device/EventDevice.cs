namespace Domain.Device;

public class EventDevice
{
    public string? IpAddress { get; set;} 
    
    public int? ChannelId { get; set; }
    
    public string? DateTime { get; set; }
    
    public string? EventType { get; set; }
    
    public string? EventState { get; set; }
    
    public string? EventDescription { get; set; }
    
    public string? DeviceId { get; set; }
    
    public string? Event_Log { get; set; }
}