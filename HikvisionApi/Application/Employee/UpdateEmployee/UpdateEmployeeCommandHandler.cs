using Application.Response;
using Domain.Employee;
using Shared.Domain.Bus;

namespace Application.Employee.UpdateEmployee;

public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, ApiResponse<string>>
{
	private readonly IEmployeeRepository _repository;

	public UpdateEmployeeCommandHandler(IEmployeeRepository repository)
	{
		_repository = repository;
	}

	public async Task<ApiResponse<string>> Handle(UpdateEmployeeCommand command)
	{
		var getEmployee = await _repository.GetEmployeeAsync(command.IdentificationNumber)
			?? throw new Exception("Employee not found");

		var isUpdated = await _repository.UpdateEmployeeAsync(new Domain.Employee.Employee()
		{
			Identification = new EmployeeId(command.IdentificationNumber),
			LastName = command.LastName,
			Name = command.Name,
			Gender = Enum.TryParse<EmployeeGender>(command.Gender, out var gender) ? gender : EmployeeGender.unknown,
			BeginTime = command.BeginDate,
			EndTime = command.EndDate,
			Status = Enum.TryParse<EmployeeStatus>(command.Status, out var status) ? status : EmployeeStatus.INACTIVE,
		});

		if (isUpdated)
		{
			return new ApiResponse<string>()
			{
				Data = command.IdentificationNumber,
				Success = true,
				StatusCode = 200,
				Message = "Employee updated successfully"
			};
		}

		return new ApiResponse<string>()
		{
			Success = false,
			StatusCode = 500,
			Message = "Error updating employee"
		};
	}
}
