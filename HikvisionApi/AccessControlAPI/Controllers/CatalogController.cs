using Application.Catalogs.EmployeeGender;
using Application.Catalogs.EmployeeStatus;
using Application.Catalogs.EmployeeType;
using Application.Response;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Query;

namespace AMPMAccesControlAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
	private readonly IQueryBus _queryBus;

	public CatalogController(IQueryBus queryBus)
	{
		_queryBus = queryBus;
	}


	[HttpGet("genders")]
	public async Task<IActionResult> GetGenders(CancellationToken cancellationToken)
	{

		var result = await _queryBus.AskAsync<ApiResponse<List<EmployeeGender>>>(new GetEmployeeGendersQuery());

		return Ok(result);
	}

	[HttpGet("statuses")]
	public async Task<IActionResult> GetStatuses(CancellationToken cancellationToken)
	{
		var result = await _queryBus.AskAsync<ApiResponse<List<EmployeeStatus>>>(
			new GetEmployeeStatusQuery()
		);


		return Ok(result);
	}

	[HttpGet("employee-types")]
	public async Task<IActionResult> GetEmployeeTypes(CancellationToken cancellationToken)
	{
		var result = await _queryBus.AskAsync<ApiResponse<List<EmployeeType>>>(
			new GetEmployeeTypesQuery()
		);

		return Ok(result);
	}
}

