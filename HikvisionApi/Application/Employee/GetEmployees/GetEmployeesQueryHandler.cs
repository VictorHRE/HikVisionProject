using Application.Employee.EmployeeDto;
using Application.Response;
using Domain.Employee;
using Shared.Domain.Query;

namespace Application.Employee.GetEmployees;

public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, ApiResponse<List<EmployeeDto.EmployeeDto>>>
{

	private readonly IEmployeeRepository _employeeRepository;

	public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
	{
		_employeeRepository = employeeRepository;
	}

	public async Task<ApiResponse<List<EmployeeDto.EmployeeDto>>> Handle(GetEmployeesQuery query)
	{

		var employees = await _employeeRepository.GetEmployeesAsync();

		if (employees.Count > 0)
		{
			return new ApiResponse<List<EmployeeDto.EmployeeDto>>()
			{
				Data = employees.Select(x => EmployeeDto.EmployeeDto.ToDto(x)).ToList(),
				Success = true,
				StatusCode = 200
			};
		}

		return new ApiResponse<List<EmployeeDto.EmployeeDto>>()
		{
			Data = new List<EmployeeDto.EmployeeDto>(),
			StatusCode = 200,
			Success = true
		};
	}
}
