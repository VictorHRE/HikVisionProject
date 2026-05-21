using Application.Response;
using Shared.Domain.Query;

namespace Application.Catalogs.EmployeeType;

public class GetEmployeeTypesQueryHandler : IQueryHandler<GetEmployeeTypesQuery, ApiResponse<List<EmployeeType>>>
{
	public Task<ApiResponse<List<EmployeeType>>> Handle(GetEmployeeTypesQuery query)
	{
		var types = new List<EmployeeType>();

		foreach (var item in Enum.GetValues(typeof(Domain.Employee.EmployeeType)))
		{
			var employeeType = new EmployeeType((int)item, $"{item}");

			types.Add(employeeType);
		}

		return Task.FromResult(new ApiResponse<List<EmployeeType>>()
		{
			Data = types,
			Message = "Success",
			Success = true,
			StatusCode = 200
		});
	}
}