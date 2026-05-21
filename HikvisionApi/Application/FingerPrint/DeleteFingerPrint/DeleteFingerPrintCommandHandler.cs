using Application.Response;
using Domain.Employee;
using Shared.Domain.Bus;

namespace Application.FingerPrint.DeleteFingerPrint;

public class DeleteFingerPrintCommandHandler : ICommandHandler<DeleteFingerPrintCommand, ApiResponse<string>>
{
	private readonly IEmployeeRepository _employeeRepository;

	public DeleteFingerPrintCommandHandler(IEmployeeRepository employeeRepository)
	{
		_employeeRepository = employeeRepository;
	}

	public async Task<ApiResponse<string>> Handle(DeleteFingerPrintCommand command)
	{
		var employee = await _employeeRepository.GetEmployeeAsync(command.IdentificationNumber)
			?? throw new Exception("Employee not found");

		var result = await _employeeRepository.DeleteFingerPrintAsync(employee, command.FingerIndex);

		if (result)
		{
			return new ApiResponse<string>()
			{
				Success = true,
				Message = "Finger print deleted successfully",
				Data = command.IdentificationNumber,
				StatusCode = 200
			};
		}

		return new ApiResponse<string>()
		{
			Success = true,
			Message = "Finger print not deleted",
			Data = command.IdentificationNumber,
			StatusCode = 400
		};
	}
}
