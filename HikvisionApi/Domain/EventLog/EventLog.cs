using Domain.Device;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.EventLog;
[Table("EventLog")]
public class EventLog {
    [Key]
    public int Id { get; set; }

    public string Data { get; set; }

    public string EventType { get; set; }

    public DateTime CreatedAt { get; set; }

    public string IdStoreHQ { get; set; }

    public string EmployeeIdentification {get; set; }

    public EventLog(string data, DateTime createdAt, string eventType, string idStoreHQ = "0",  string employeeIdentification = "")
    {
        Data = data;
        CreatedAt = createdAt;
        EventType = eventType;
        IdStoreHQ = idStoreHQ;
        EmployeeIdentification = employeeIdentification;
    }
    

    public EventLog()
    {
        Data = string.Empty;
        CreatedAt = DateTime.Now;
        EventType = string.Empty;
        IdStoreHQ = "0";
        EmployeeIdentification  = string.Empty;
    }

}