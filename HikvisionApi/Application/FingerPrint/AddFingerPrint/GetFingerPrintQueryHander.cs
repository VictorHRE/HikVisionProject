using Application.Response;
using Azure;
using Domain.Employee;
using Shared.Domain.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FingerPrint.AddFingerPrint;

public class GetFingerPrintQueryHander : IQueryHandler<GetFingerPrintQuery, ApiResponse<CaptureFingerPrintDto>?>
{
	private readonly IEmployeeRepository _employeeRepository;

	public GetFingerPrintQueryHander(IEmployeeRepository employeeRepository)
	{
		_employeeRepository = employeeRepository;
	}

	public async Task<ApiResponse<CaptureFingerPrintDto>?> Handle(GetFingerPrintQuery query)
	{

		try
		{
			var employee = await _employeeRepository.GetEmployeeAsync(query.EmployeeNo);

			if (employee == null)
			{
				return new()
				{
					Data = null,
					Success = false,
					Message = "The employee that you are trying to capture the fingerprint does not exist",
					StatusCode = 404
				};
			}

			var fingerPrint = await _employeeRepository.AddFingerPrintAsync(employee, query.FingerNo);

			var response = new CaptureFingerPrintDto()
			{
				FingerData = fingerPrint.FingerData,
				FingerNo = fingerPrint.FingerNo,
				FingerPrintQuality = fingerPrint.FingerPrintQuality,
				TotalStatus = fingerPrint.Status.totalStatus,
				Message = fingerPrint.Status.StatusList.FirstOrDefault()?.errorMsg ?? ""
			};

			return new()
			{
				Data = response,
				Success = true,
				Message = "Fingerprint captured successfully",
				StatusCode = 200
			};
		} catch (Exception ex)
		{
			return new()
			{
				Success = false,
				Message = ex.Message,
				StatusCode = 400
			};
		}
	}
}
