using Application.Response;
using Shared.Domain.Query;

namespace Application.Catalogs.EmployeeStatus;
public class GetEmployeeStatusQueryHandler : IQueryHandler<GetEmployeeStatusQuery, ApiResponse<List<EmployeeStatus>>>
{
	public Task<ApiResponse<List<EmployeeStatus>>> Handle(GetEmployeeStatusQuery query)
	{
		var employeeStatus = new List<EmployeeStatus>();

		foreach (var item in Enum.GetValues(typeof(Domain.Employee.EmployeeStatus)))
		{
			var status = new EmployeeStatus((int)item, $"{item}");

			employeeStatus.Add(status);
		}
		
		return Task.FromResult(new ApiResponse<List<EmployeeStatus>>
		{
			Data = employeeStatus,
			Message = "Success",
			Success = true,
			StatusCode = 200
		});
	}
}
