namespace Domain.Employee;

public class EmployeeFingerPrintStatus
{

	public string status { get; set; } = string.Empty;
	public List<StatusListItem> StatusList { get; set; } = [];
	public int totalStatus { get; set; }
}

public class StatusListItem
{
	public int? id { get; set; }
	public int? cardReaderRecvStatus { get; set; }
	public string errorMsg { get; set; } = string.Empty;
}
