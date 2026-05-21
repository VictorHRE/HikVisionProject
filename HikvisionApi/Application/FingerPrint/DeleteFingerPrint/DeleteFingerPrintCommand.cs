using Shared.Domain.Bus;

namespace Application.FingerPrint.DeleteFingerPrint;

public class DeleteFingerPrintCommand: Command
{
	public string IdentificationNumber { get; set; } = string.Empty;
	public int FingerIndex { get; set; }
}
