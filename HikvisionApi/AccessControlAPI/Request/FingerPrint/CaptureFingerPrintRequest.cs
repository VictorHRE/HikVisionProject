using System.ComponentModel.DataAnnotations;

namespace AMPMAccesControlAPI.Request.FingerPrint;

public class CaptureFingerPrintRequest
{

	[RegularExpression(@"^\d{3}-\d{6}-\d{4}[A-Za-z]$",
		ErrorMessage = "El número de identificación no tiene un formato válido.")]
	public string IdentificationNumber { get; set; } = string.Empty;

	[Range(1, 10, ErrorMessage = "El indice de dedo debe estar entre 1 y 10")]
	public int FingerIndex { get; set; } = 1;
}
