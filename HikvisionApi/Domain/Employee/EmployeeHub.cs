using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Employee;
[Table("EmployeeHub")]
public class EmployeeHub {
    [Key]
    public int Id { get; set; }

    public string Identification { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public string? Position { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? IdStoreHQ { get; set; }//idStore

    public string Status { get; set; }

    public string? UserType { get; set; }

    public string? Gender { get; set; }

    public DateTime BeginTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime BirthDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Finger { get; set; }

}