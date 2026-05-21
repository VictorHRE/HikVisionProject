using Shared.Domain.Bus;

namespace Application.Employee.DeleteEmployee;

public class DeleteEmployeeCommand(string identificationNumber) : Command
{
	public string IdentificationNumber { get; set; } = identificationNumber;
}
