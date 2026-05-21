using System.Xml.Serialization;

namespace Domain.Employee;

[XmlRoot("CaptureFingerPrint", Namespace = "http://www.isapi.org/ver20/XMLSchema")]
public class EmployeeFingerPrint
{
	[XmlElement("fingerData")]
	public string FingerData { get; set; } = string.Empty;

	[XmlElement("fingerNo")]
	public int FingerNo { get; set; }

	[XmlElement("fingerPrintQuality")]
	public int FingerPrintQuality { get; set; }

	public EmployeeFingerPrintStatus Status { get; set; } = new();
}