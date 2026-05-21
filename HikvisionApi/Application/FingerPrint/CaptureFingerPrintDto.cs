using System.Xml.Serialization;

namespace Application.FingerPrint;

public class CaptureFingerPrintDto
{
	public string FingerData { get; set; } = string.Empty;

	public int FingerNo { get; set; }

	public int FingerPrintQuality { get; set; }

	public string Message { get; set; } = string.Empty;

	public int TotalStatus { get; set; }
}
