using Domain.Employee;
using Infrastructure.HttpClients.DigestClient;
using Infrastructure.ISAPI;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Xml.Linq;

namespace Infrastructure.Repositories;

public class EmployeeRepository(
	DigestAuthClient digestAuthClient,
	AmpmAccessControlContext context) : IEmployeeRepository
{
	public async Task<string> AddEmployeeAsync(Employee employee)
	{
		//var now = DateTime.Now;
		var beginTime = DateTime.Today.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss"); //employee.BeginTime.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss");

		//var tenYearsLater = now.AddYears(10);
		var endTime = employee.EndTime.ToString("yyyy-MM-ddTHH:mm:ss");

		var newEmployee = new
		{
			UserInfo = new
			{
				employeeNo = employee.Identification.Value.Replace("-", ""),
				name = $"{employee.Name} {employee.LastName}",
				userType = nameof(EmployeeType.normal),
				Valid = new
				{
					enable = true,
					beginTime,
					endTime,
					timeType = "local"
				},
				doorRight = "1",
				gender = "male",
				//userVerifyMode = "fp",
				checkUser = true,
				operateType = "byTerminal",
				RightPlan = new[]
				{
					new { doorNo = 1, planTemplateNo = "1" }
				},
				groupId = 1
			}
		};

		var jsonBody = JsonSerializer.Serialize(newEmployee);

		var response = await digestAuthClient
			.PostDigestAuthAsync(
				Routes.CreateEmployee,
				jsonBody,
				"application/json"
			);

		var respBody = await response.Content.ReadAsStringAsync();

		var result = JsonSerializer.Deserialize<ISAPIResponseStatus>(respBody);

		if (response.IsSuccessStatusCode)
		{
			if (result?.statusString == "OK")
			{
				return result.statusString!;
			}
		}

		return result!.subStatusCode == "employeeNoAlreadyExist"
			? throw new Exception("El empleado ya se encuentra agregado a esta sucursal")
			: string.Empty;
	}

	public async Task AddEmployeeDbAsync(Employee employee)
	{
		await context.Employees.AddAsync(employee);
	}

	public async Task<string> DeleteEmployeeAsync(Employee employee)
	{
		/*var deleteEmployee = new
        {
            UserInfoDelCond = new
            {
                EmployeeNoList = new[]
                {
                    new { employeeNo = employeeNumber }
                }
            }
        };

        string jsonBody = JsonSerializer.Serialize(deleteEmployee);

        var response = await _digestAuthClient
                            .PutDigestAuthAsync(
                                "ISAPI/AccessControl/UserInfo/Delete?format=json",
                                jsonBody,
                                "application/json"
                            );*/

		/*var deleteEmployeeAllInfo = new
        {
            UserInfoDetail = new
            {
                mode = "byEmployeeNo",
                EmployeeNoList = new[]
                {
                    new { employeeNo = identificationNumber }
                }
            }
        };

        string jsonBodyAll = JsonSerializer.Serialize(deleteEmployeeAllInfo);

        var response = await _digestAuthClient
            .PutDigestAuthAsync(
                Routes.DeleteEmployee,
                jsonBodyAll,
                "application/json"
            );

        var respBody = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ISAPIResponseStatus>(respBody);

        if (result?.statusString == "OK")
        {
            return result.statusString!;
        }

        return string.Empty;*/

		var endTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

		var updateEmployee = new
		{
			UserInfo = new
			{
				employeeNo = employee.Identification.Value.Replace("-", ""),
				name = employee.Name,
				userType = EmployeeType.blackList.ToString(),
				Valid = new
				{
					enable = true,
					beginTime = employee.BeginTime.ToString("yyyy-MM-ddTHH:mm:ss"),
					endTime = endTime,
					timeType = "local"
				},
				gender = employee.Gender.ToString()
			}
		};

		string jsonBody = JsonSerializer.Serialize(updateEmployee);

		var response = await digestAuthClient
			.PutDigestAuthAsync(
				Routes.UpdateEmployee,
				jsonBody,
				"application/json"
			);

		var respBody = await response.Content.ReadAsStringAsync();

		var result = JsonSerializer.Deserialize<ISAPIResponseStatus>(respBody);

		if (result?.statusString == "OK")
		{
			employee.Status = EmployeeStatus.DELETED;
			context.Employees.Update(employee);

			return result.statusString!;
		}

		throw new Exception($"Error deleting employee: {respBody}");
	}

	public async Task DeleteEmployeeDbAsync(Employee employee)
	{
		var endTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

		var updateEmployee = new
		{
			UserInfo = new
			{
				employeeNo = employee.Identification.Value.Replace("-", ""),
				name = employee.Name,
				userType = EmployeeType.blackList.ToString(),
				Valid = new
				{
					enable = true,
					beginTime = employee.BeginTime.ToString("yyyy-MM-ddTHH:mm:ss"),
					endTime = endTime,
					timeType = "local"
				},
				gender = employee.Gender.ToString()
			}
		};

		string jsonBody = JsonSerializer.Serialize(updateEmployee);

		var response = await digestAuthClient
			.PutDigestAuthAsync(
				Routes.UpdateEmployee,
				jsonBody,
				"application/json"
			);

		var respBody = await response.Content.ReadAsStringAsync();

		var result = JsonSerializer.Deserialize<ISAPIResponseStatus>(respBody);

		if (result?.statusString == "OK")
		{
			employee.Status = EmployeeStatus.DELETED;
			context.Employees.Update(employee);
			return;
		}

		throw new Exception($"Error deleting employee: {respBody}");
	}

	public async Task<Employee?> GetEmployeeAsync(string identificationNumber)
	{
		var serchEmployee = new
		{
			UserInfoSearchCond = new
			{
				searchID = "1",
				searchResultPosition = 0,
				maxResults = 10,
				EmployeeNoList = new[]
				{
					new { employeeNo = identificationNumber.Replace("-", "") }
				}
			}
		};

		string jsonBody = JsonSerializer.Serialize(serchEmployee);

		var response = await digestAuthClient
			.PostDigestAuthAsync(
				Routes.ListEmployees,
				jsonBody,
				"application/json"
			);

		var respBody = await response.Content.ReadAsStringAsync();

		var result = JsonSerializer.Deserialize<UserInfoSearchResponse>(respBody);

		if (result?.UserInfoSearch is not null && result?.UserInfoSearch?.responseStatusStrg == "OK")
		{
			var employee = result.UserInfoSearch.UserInfo.FirstOrDefault();

			if (employee is null) return null;

			var query = await context.Employees.AsNoTracking()
				.ToListAsync();


			var employeeDb = query.FirstOrDefault(x =>
				x.Identification.Value.Replace("-", "") == employee.employeeNo.Replace("-", ""));

			if (employeeDb is null) return null;

			var gender = Enum.TryParse<EmployeeGender>(employee.gender, ignoreCase: true, out var _gender)
				? _gender
				: EmployeeGender.unknown;

			var userTypes = Enum.TryParse<EmployeeType>(employee.userType, ignoreCase: true, out var _userType)
				? _userType
				: EmployeeType.unknown;

			var status = DateTime.Parse(employee.Valid.endTime) > DateTime.Now
				? EmployeeStatus.ACTIVE
				: EmployeeStatus.EXPIRED;

			return new Employee(
				id: employeeDb.Id,
				identification: employeeDb.Identification,
				name: employee.name,
				lastName: employeeDb.LastName,
				position: employeeDb.Position,
				email: employeeDb.Email,
				phone: employeeDb.Phone,
				branchId: employeeDb.BranchId,
				userType: userTypes,
				status: status,
				gender: gender,
				beginTime: DateTime.Parse(employee.Valid.beginTime),
				endTime: DateTime.Parse(employee.Valid.endTime),
				birthDate: employeeDb.BirthDate,
				createdAt: employeeDb.CreatedAt
			);
		}

		return null;
	}

	public async Task<List<Employee>> GetEmployeesAsync()
	{
		var serchEmployee = new
		{
			UserInfoSearchCond = new
			{
				searchID = "1",
				searchResultPosition = 0,
				maxResults = 1000
			}
		};

		string jsonBody = JsonSerializer.Serialize(serchEmployee);

		var response = await digestAuthClient
			.PostDigestAuthAsync(
				Routes.ListEmployees,
				jsonBody,
				"application/json"
			);

		var respBody = await response.Content.ReadAsStringAsync();

		var result = JsonSerializer.Deserialize<UserInfoSearchResponse>(respBody);

		if (result?.UserInfoSearch is not null && result?.UserInfoSearch?.responseStatusStrg == "OK")
		{
			var employees = result.UserInfoSearch.UserInfo.ToList();

			if (employees.Count == 0) return [];

			var employeesDb = await context.Employees.ToListAsync();

			var mergedEmployees = from emp in employees
								  join empDb in employeesDb
									  on emp.employeeNo equals empDb.Identification.Value.Replace("-", "")
								  select new Employee(
									  id: empDb.Id,
									  identification: empDb.Identification,
									  name: empDb.Name,
									  lastName: empDb.LastName,
									  position: empDb.Position,
									  email: empDb.Email,
									  phone: empDb.Phone,
									  branchId: empDb.BranchId,
									  status: DateTime.Parse(emp.Valid.endTime) > DateTime.Now ? empDb.Status : EmployeeStatus.EXPIRED,
									  userType: empDb.UserType,
									  gender: empDb.Gender ?? EmployeeGender.unknown,
									  beginTime: DateTime.Parse(emp.Valid.beginTime),
									  endTime: DateTime.Parse(emp.Valid.endTime),
									  birthDate: empDb.BirthDate,
									  createdAt: empDb.CreatedAt
								  );


			return [.. mergedEmployees];
		}

		return [];
	}

	public async Task<EmployeeFingerPrint> AddFingerPrintAsync(Employee employee, int fingerIndex = 1)
	{
		if (fingerIndex < 1 || fingerIndex > 10)
		{
			throw new ArgumentOutOfRangeException(nameof(fingerIndex), "fingerIndex must be between 1 and 10.");
		}

		var xml = @$"<CaptureFingerPrintCond version=""2.0"" xmlns=""http://www.isapi.org/ver20/XMLSchema"">
						<fingerNo>{fingerIndex}</fingerNo> <!-- Debes reemplazar ""1"" con el número de dedo real (1-10) -->
					</CaptureFingerPrintCond>";

		var response = await digestAuthClient
			.PostDigestAuthAsync(
				Routes.CaptureFingerPrint,
				xml,
				"application/xml"
			);


		if (response.IsSuccessStatusCode)
		{
			var xmlString = await response.Content.ReadAsStringAsync();

			var xmlDoc = XDocument.Parse(xmlString);
			XNamespace ns = "http://www.isapi.org/ver20/XMLSchema";

			var fingerData = xmlDoc.Root!.Element(ns + "fingerData")?.Value!;
			var fingerNo = xmlDoc.Root.Element(ns + "fingerNo")?.Value!;
			var fingerPrintQuality = xmlDoc.Root.Element(ns + "fingerPrintQuality")?.Value!;

			//in case of error
			if (fingerData is null || fingerNo is null || fingerPrintQuality is null)
			{
				ns = "http://www.std-cgi.org/ver20/XMLSchema";
				var requestURL = xmlDoc.Root.Element(ns + "requestURL")?.Value;
				var statusCode = xmlDoc.Root.Element(ns + "statusCode")?.Value;
				var statusString = xmlDoc.Root.Element(ns + "statusString")?.Value;
				var subStatusCode = xmlDoc.Root.Element(ns + "subStatusCode")?.Value;
				var mErrCode = xmlDoc.Root.Element(ns + "MErrCode")?.Value;
				var mErrDevSelfEx = xmlDoc.Root.Element(ns + "MErrDevSelfEx")?.Value;

				throw new Exception(@$"Error: {requestURL} - 
											{statusCode} - {statusString} - {subStatusCode} - {mErrCode} - {mErrDevSelfEx}");
			}

			var employeeFingerPrint = new EmployeeFingerPrint()
			{
				FingerData = fingerData,
				FingerNo = int.Parse(fingerNo),
				FingerPrintQuality = int.Parse(fingerPrintQuality)
			};

			var employeeFingerPrintCfg = new
			{
				FingerPrintCfg = new
				{
					employeeNo = employee.Identification.Value.Replace("-", ""),
					enableCardReader = new[] { 1 },
					fingerPrintID = fingerIndex,
					fingerType = "normalFP",
					fingerData = employeeFingerPrint.FingerData,
					checkEmployeeNo = true
				}
			};

			string jsonBody = JsonSerializer.Serialize(employeeFingerPrintCfg);

			var request = await digestAuthClient
				.PostDigestAuthAsync(
					Routes.SetFingerPrint,
					jsonBody,
					"application/json"
				);

			var respBody = await request.Content.ReadAsStringAsync();

			var responseFingerPrint = JsonSerializer.Deserialize<EmployeeFingerPrintStatus>(respBody);

			employeeFingerPrint.Status = responseFingerPrint!;

			if (request.IsSuccessStatusCode) return employeeFingerPrint;
		}

		if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
		{
			throw new Exception("unable to capture fingerprint");
		}

		throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
	}

	public async Task<bool> UpdateEmployeeAsync(Employee employee)
	{
		var updateEmployee = new
		{
			UserInfo = new
			{
				employeeNo = employee.Identification.Value.Replace("-", ""),
				name = employee.Name,
				userType = employee.Status == EmployeeStatus.INACTIVE ? EmployeeType.blackList.ToString() : EmployeeType.normal.ToString(),
				Valid = new
				{
					enable = true,
					beginTime = employee.BeginTime.ToString("yyyy-MM-ddTHH:mm:ss"),
					endTime = employee.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
					timeType = "local"
				},
				gender = employee.Gender.ToString()
			}
		};

		string jsonBody = JsonSerializer.Serialize(updateEmployee);

		var employeeDbs = await context.Employees.ToListAsync();

		var employeeDb = employeeDbs
							 .FirstOrDefault(x =>
								 x.Identification.Value.Replace("-", "") ==
								 employee.Identification.Value.Replace("-", ""))
						 ?? throw new Exception("Employee not found");

		var response = await digestAuthClient
			.PutDigestAuthAsync(
				Routes.UpdateEmployee,
				jsonBody,
				"application/json"
			);

		var respBody = await response.Content.ReadAsStringAsync();

		var result = JsonSerializer.Deserialize<ISAPIResponseStatus>(respBody);

		if (result?.statusString == "OK")
		{
			employeeDb.Name = employee.Name;
			employeeDb.LastName = employee.LastName;
			employeeDb.Identification = employee.Identification;
			employeeDb.Status = employee.Status;
			//employeeDb.UserType = employee.UserType;
			employeeDb.Email = employee.Email;
			employeeDb.Phone = employee.Phone;
			employeeDb.Position = employee.Position;
			employeeDb.BranchId = employee.BranchId;
			employeeDb.BeginTime = employee.BeginTime;
			employeeDb.EndTime = employee.EndTime;
			employeeDb.BirthDate = employee.BirthDate;
			employeeDb.Gender = employee.Gender;

			context.Employees.Update(employeeDb);

			return true;
		}

		if (result?.subStatusCode == "employeeNoNotExist")
		{
			throw new Exception("Employee not found");
		}

		throw new Exception($"Error: {result?.statusString} - {result?.subStatusCode}");
	}

	public async Task<bool> DeleteFingerPrintAsync(Employee employee, int fingerIndex = 1)
	{
		if (fingerIndex < 1 || fingerIndex > 10)
		{
			throw new ArgumentOutOfRangeException(nameof(fingerIndex), "fingerIndex must be between 1 and 10.");
		}

		//_ = await GetEmployeeAsync(employee.Identification.Value!) ?? throw new Exception("Employee not found");

		var deleteFingerPrint = new
		{
			FingerPrintDelete = new
			{
				mode = "byEmployeeNo",
				EmployeeNoDetail = new
				{
					employeeNo = employee.Identification.Value.Replace("-", ""),
					//enableCardReader = new[] { 1 },
					fingerPrintID = fingerIndex
				}
			}
		};

		string jsonBody = JsonSerializer.Serialize(deleteFingerPrint);

		var response = await digestAuthClient
			.PutDigestAuthAsync(
				Routes.DeleteFingerPrint,
				jsonBody,
				"application/json"
			);

		var respBody = await response.Content.ReadAsStringAsync();

		var result = JsonSerializer.Deserialize<ISAPIResponseStatus>(respBody);

		if (result?.statusString == "OK")
		{
			return true;
		}

		if (result?.subStatusCode == "employeeNoNotExist")
		{
			throw new Exception("Employee not found");
		}

		throw new Exception($"Error: {result?.statusString} - {result?.subStatusCode}");
	}
}