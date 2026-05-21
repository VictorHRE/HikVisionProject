
using Domain.Employee;
using Shared.Domain.Bus;

namespace Application.Employee.CreateEmployee;

public class CreateEmployeeCommand : Command

{
	public string Identification { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string? Email { get; set; }

	public string? Position { get; set; } = string.Empty;

	public string? Phone { get; set; } = string.Empty;

	public int BranchId { get; set; }

	public string Status { get; set; } = string.Empty;

	public string? Gender { get; set; } = string.Empty;

	public DateTime BeginDate { get; set; }

	public DateTime EndDate { get; set; }

	public DateTime BirthDate { get; set; }

}
