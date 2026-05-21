using Application.Response;
using Domain.Employee;
using Shared.Domain.Bus;
using Infrastructure.UnitOfWork;

namespace Application.Employee.CreateEmployee;

public class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand, ApiResponse<string>>
{
	private readonly IEmployeeRepository _employeeRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
	{
		_employeeRepository = employeeRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<ApiResponse<string>> Handle(CreateEmployeeCommand command)
	{
		var wasCreatedDevice = false;
		var wasCreatedDb = false;
		Domain.Employee.Employee employeeCreaed = null;


		try
		{
			var newEmployee = Domain.Employee.Employee.Create(
				id: 0,
				identification: new EmployeeId(command.Identification),
				name: command.Name,
				lastName: command.LastName,
				position: command.Position ?? string.Empty,
				email: command.Email ?? string.Empty,
				phone: command.Phone ?? string.Empty,
				branchId: command.BranchId,
				status: Enum.TryParse<EmployeeStatus>(command.Status, out var result) ? result : EmployeeStatus.ACTIVE,
				userType: EmployeeType.normal,
				gender: Enum.TryParse<EmployeeGender>(command.Gender, out var gender) ? gender : EmployeeGender.male,
				beginDate: command.BeginDate,
				endDate: command.EndDate,
				birthDate: command.BirthDate,
				createdAt: DateTime.Now
			);

			employeeCreaed = newEmployee;

			var employeeNo = await _employeeRepository.AddEmployeeAsync(newEmployee);

			if (string.IsNullOrEmpty(employeeNo))
			{
				throw new Exception("Failed to create employee");
			}

			wasCreatedDevice = true;

			await _unitOfWork.EmployeeRepository.AddEmployeeDbAsync(newEmployee);

			await _unitOfWork.CommitAsync(cancellationToken: default);

			wasCreatedDb = true;

			return new()
			{
				Data = employeeNo,
				StatusCode = 200,
				Success = true,
				Message = "Employee created successfully"
			};
		}
		catch (Exception ex)
		{
			if (wasCreatedDb && employeeCreaed is not null)
			{
				await _unitOfWork.EmployeeRepository.DeleteEmployeeDbAsync(employeeCreaed);
			}

			if (wasCreatedDevice && employeeCreaed is not null)
			{
				var employee = await _unitOfWork.EmployeeRepository.GetEmployeeAsync(employeeCreaed.Identification.Value);

				if (employee is not null)
				{
					await _unitOfWork.EmployeeRepository.DeleteEmployeeAsync(employeeCreaed);
				}
			}

			return new()
			{
				Data = string.Empty,
				StatusCode = 500,
				Success = false,
				Message = $"Failed to create employee {ex.Message}"
			};
		}
	}
}