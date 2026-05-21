using System.ComponentModel.DataAnnotations;

namespace AMPMAccesControlAPI.Request.Employee;

public class UpdateEmployeeRequest
{
	[RegularExpression(@"^\d{3}-\d{6}-\d{4}[A-Za-z]$",
		ErrorMessage = "El número de identificación no tiene un formato válido.")]
	public string IdentificationNumber { get; set; } = string.Empty;

	[Required(ErrorMessage = "El campo 'Name' es obligatorio.")]
	public string Name { get; set; } = string.Empty;

	[Required(ErrorMessage = "El campo 'LastName' es obligatorio.")]
	public string LastName { get; set; } = string.Empty;

	[EmailAddress(ErrorMessage = "El formato del email no es válido")]
	public string? Email { get; set; }


	public string? Position { get; set; } = string.Empty;


	public string? Phone { get; set; } = string.Empty;

	[Required(ErrorMessage = "El campo 'BranchId' es obligatorio.")]
	public int BranchId { get; set; }

	[Required(ErrorMessage = "El campo 'Status' es obligatorio.")]
	public string Status { get; set; } = string.Empty;


	public string? Gender { get; set; } = string.Empty;

	[Required(ErrorMessage = "El campo 'BirthDate' es obligatorio.")]
	public string BirthDate { get; set; }

	[Required(ErrorMessage = "El campo 'BeginTime' es obligatorio.")]
	public string BeginDate { get; set; } 

	[Required(ErrorMessage = "El campo 'EndTime' es obligatorio.")]
	public string EndDate { get; set; }
}