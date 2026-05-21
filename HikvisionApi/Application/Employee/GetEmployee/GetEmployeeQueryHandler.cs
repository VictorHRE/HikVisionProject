using Shared.Domain.Query;
using Domain.Employee;
using Application.Response;

namespace Application.Employee.GetEmployee;

public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery, ApiResponse<EmployeeDto.EmployeeDto?>>
{
	private readonly IEmployeeRepository _employeeRepository;

	public GetEmployeeQueryHandler(IEmployeeRepository employeeRepository)
	{
		_employeeRepository = employeeRepository;
	}

	public async Task<ApiResponse<EmployeeDto.EmployeeDto?>> Handle(GetEmployeeQuery query)
	{
		var employe = await _employeeRepository.GetEmployeeAsync(query.IdentificationNumber);

		if (employe is null)
		{
			return new()
			{
				Data = null,
				StatusCode = 404,
				Success = false
			};
		}

		return new()
		{
			Data =  EmployeeDto.EmployeeDto.ToDto(employe),
			Success = true,
			StatusCode = 200
		};

	}
}
