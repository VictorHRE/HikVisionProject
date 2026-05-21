using Shared.Domain.Query;

namespace Application.FingerPrint.AddFingerPrint;

public class GetFingerPrintQuery : Query
{
	public string EmployeeNo { get; set; }

	public int FingerNo { get; set; }

	public GetFingerPrintQuery(string employeeNo, int fingerNo)
	{
		EmployeeNo = employeeNo;
		FingerNo = fingerNo;
	}
}
