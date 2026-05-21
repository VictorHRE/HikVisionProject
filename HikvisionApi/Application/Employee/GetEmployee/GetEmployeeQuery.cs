
using Shared.Domain.Query;

namespace Application.Employee.GetEmployee;

public class GetEmployeeQuery : Query
{
	public string IdentificationNumber { get; set; }

	public GetEmployeeQuery(string identificationNumber)
	{
		IdentificationNumber = identificationNumber;
	}

	public GetEmployeeQuery()
	{
		IdentificationNumber = string.Empty;
	}
}
