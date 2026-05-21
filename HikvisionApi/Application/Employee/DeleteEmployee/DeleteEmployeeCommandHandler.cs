
using Application.Response;
using Domain.Employee;
using Infrastructure.UnitOfWork;
using Shared.Domain.Bus;

namespace Application.Employee.DeleteEmployee;

public class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand, ApiResponse<string>>
{

	private readonly IEmployeeRepository _employeeRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
	{
		_employeeRepository = employeeRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<ApiResponse<string>> Handle(DeleteEmployeeCommand command)
	{
		var employee = await _employeeRepository.GetEmployeeAsync(command.IdentificationNumber)
			?? throw new Exception($"Employee {command.IdentificationNumber} not found");

		if (employee is not null)
		{
			await _unitOfWork.EmployeeRepository.DeleteEmployeeDbAsync(employee);

			await _unitOfWork.CommitAsync(cancellationToken: CancellationToken.None);

			return new ApiResponse<string>()
			{
				Success = true,
				Message = $"Employee {command.IdentificationNumber} deleted successfully",
				Data = command.IdentificationNumber,
				StatusCode = 200
			};
		}

		return new ApiResponse<string>()
		{
			Success = false,
			Message = $"Employee {command.IdentificationNumber} not found",
			Data = null,
			StatusCode = 404
		};
	}
}