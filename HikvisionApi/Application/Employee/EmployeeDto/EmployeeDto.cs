
namespace Application.Employee.EmployeeDto;

public class EmployeeDto
{
	public int Id { get; set; }

	public string IdentificationNumber { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string? Email { get; set; }

	public string Position { get; set; } = string.Empty;

	public string Phone { get; set; } = string.Empty;

	public string Status { get; set; } = string.Empty;

	public int BranchId { get; set; }

	public string UserType { get; set; } = string.Empty;

	public DateTime BeginTime { get; set; }

	public DateTime EndTime { get; set; }

	public string Gender { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; }

	public DateTime BirthDate { get; set; }


	public static EmployeeDto ToDto(Domain.Employee.Employee employee)
	{
		return new EmployeeDto
		{
			Id = employee.Id,
			IdentificationNumber = employee.Identification.Value,
			Name = employee.Name,
			LastName =  employee.LastName,
			Email = employee.Email,
			Position = employee.Position,
			Status = employee.Status.ToString(),
			Phone = employee.Phone,
			BranchId = employee.BranchId,
			UserType = employee.UserType.ToString(),
			Gender = employee.Gender.ToString(),
			BeginTime = employee.BeginTime,
			EndTime = employee.EndTime,
			BirthDate = employee.BirthDate,
			CreatedAt = employee.CreatedAt
		};
	}
}
