using Application.Response;
using Shared.Domain.Query;

namespace Application.Catalogs.EmployeeGender;

public class GetEmployeeGendersQueryHandler : IQueryHandler<GetEmployeeGendersQuery, ApiResponse<List<EmployeeGender>>>
{
	public Task<ApiResponse<List<EmployeeGender>>> Handle(GetEmployeeGendersQuery query)
	{

		var genders = new List<EmployeeGender>();

		foreach (var item in Enum.GetValues(typeof(Domain.Employee.EmployeeGender)))
		{
			var gender = new EmployeeGender((int)item, $"{item}");

			genders.Add(gender);
		}

		return Task.FromResult(
			new ApiResponse<List<EmployeeGender>>
			{
				Data = genders,
				Message = "Success",
				Success = true,
				StatusCode = 200
			}
		);
	}

}