using System.Text.Json;
using AMPMAccesControlAPI.Request.Employee;
using AMPMAccesControlAPI.Request.FingerPrint;
using Application.Employee.CreateEmployee;
using Application.Employee.DeleteEmployee;
using Application.Employee.EmployeeDto;
using Application.Employee.GetEmployee;
using Application.Employee.GetEmployees;
using Application.Employee.UpdateEmployee;
using Application.FingerPrint;
using Application.FingerPrint.AddFingerPrint;
using Application.FingerPrint.DeleteFingerPrint;
using Application.Response;
using Infrastructure.HttpClients.CentralHubClient;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Bus;
using Shared.Domain.Query;

namespace AMPMAccesControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(
        ICommandBus commandBus,
        IQueryBus queryBus,
        ICentralApiHttpClient centralApiHttpClient) : ControllerBase
    {
        private readonly IQueryBus _queryBus = queryBus;
        private readonly ICommandBus _commandBus = commandBus;

        [HttpGet("get-token")]
        public async Task<IActionResult> GetToken()
        {
            var request = await centralApiHttpClient.SendPostAsync(
                "eventlog/add-eventlog",
                JsonSerializer.Serialize(new
                {
                    Id = 0,
                    data = "admin",
                    eventType = "qwpassword",
                    createdAt = DateTime.Now,
                    idStoreHQ = "AMPMTEST"
                }));

            var responseBody = await request.Content.ReadAsStringAsync();

            return Ok(new { Token = responseBody });
        }


        [HttpPost("get-employee-by-id")]
        public async Task<ActionResult> GetEmployeeById([FromBody] string employeeNo)
        {
            try
            {
                var response = await _queryBus.AskAsync<ApiResponse<EmployeeDto>>(new GetEmployeeQuery(employeeNo));

                if (!response.Success)
                {
                    return StatusCode(response.StatusCode, response.Message);
                }

                return Ok(response.SerializableResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("get-employees")]
        public async Task<ActionResult> GetEmployees()
        {
            try
            {
                var response = await _queryBus.AskAsync<ApiResponse<List<EmployeeDto>>>(new GetEmployeesQuery());

                return Ok(response.SerializableResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("add-employee")]
        public async Task<ActionResult> AddEmployee([FromBody] CreateEmployeeRequest employeeRequest)
        {
            try
            {
                var response = await _commandBus.DispatchAsync<ApiResponse<string>>(new CreateEmployeeCommand()
                {
                    Identification = employeeRequest.IdentificationNumber.ToUpper(),
                    Name = employeeRequest.Name,
                    LastName = employeeRequest.LastName,
                    Email = employeeRequest.Email,
                    Position = employeeRequest.Position,
                    Phone = employeeRequest.Phone,
                    Status = employeeRequest.Status,
                    BranchId = employeeRequest.BranchId,
                    BeginDate = DateTime.Parse(employeeRequest.BeginDate),
                    EndDate = DateTime.Parse(employeeRequest.EndDate),
                    BirthDate = DateTime.Parse(employeeRequest.BirthDate),
                    Gender = employeeRequest.Gender
                });

                return Ok(response.SerializableResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("update-employee")]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest employeeRequest)
        {
            try
            {
                var response = await _commandBus.DispatchAsync<ApiResponse<string>>(new UpdateEmployeeCommand()
                {
                    IdentificationNumber = employeeRequest.IdentificationNumber.ToUpper(),
                    Name = employeeRequest.Name,
                    LastName = employeeRequest.LastName,
                    Email = employeeRequest.Email,
                    Phone = employeeRequest.Phone,
                    Status = employeeRequest.Status,
                    Position = employeeRequest.Position,
                    BranchId = employeeRequest.BranchId,
                    Gender = employeeRequest.Gender,
                    BeginDate = DateTime.Parse(employeeRequest.BeginDate),
                    EndDate = DateTime.Parse(employeeRequest.EndDate),
                    BirthDate = DateTime.Parse(employeeRequest.BirthDate),
                });

                return Ok(response.SerializableResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete-employee")]
        public async Task<ActionResult> DeleteEmployee([FromBody] string identificationNumber)
        {
            try
            {
                var response =
                    await _commandBus.DispatchAsync<ApiResponse<string>>(
                        new DeleteEmployeeCommand(identificationNumber));

                return StatusCode(response.StatusCode, response.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("add-finger-print")]
        public async Task<ActionResult> AddFingerPrint([FromBody] CaptureFingerPrintRequest captureFingerPrintRequest)
        {
            try
            {
                var verifiEmployee = await _queryBus
                    .AskAsync<ApiResponse<EmployeeDto>>(new GetEmployeeQuery()
                    {
                        IdentificationNumber = captureFingerPrintRequest.IdentificationNumber
                    });


                if (!verifiEmployee.Success)
                {
                    return StatusCode(verifiEmployee.StatusCode, verifiEmployee.Message);
                }

                var request = await _queryBus
                    .AskAsync<ApiResponse<CaptureFingerPrintDto>>(new GetFingerPrintQuery(
                        captureFingerPrintRequest.IdentificationNumber,
                        captureFingerPrintRequest.FingerIndex)
                    );

                if (!request.Success)
                {
                    return StatusCode(request.StatusCode, request.Message);
                }

                return Ok(request.SerializableResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("delete-finger-print")]
        public async Task<ActionResult> DeleteFingerPrint(
            [FromBody] CaptureFingerPrintRequest captureFingerPrintRequest)
        {
            try
            {
                var response = await _commandBus.DispatchAsync<ApiResponse<string>>(new DeleteFingerPrintCommand()
                {
                    IdentificationNumber = captureFingerPrintRequest.IdentificationNumber,
                    FingerIndex = captureFingerPrintRequest.FingerIndex
                });

                return StatusCode(response.StatusCode, response.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}