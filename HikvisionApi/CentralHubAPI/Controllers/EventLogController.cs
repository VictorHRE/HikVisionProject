using System;
using AMPM_CentralHubAPI.Request.EventLog;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.EventLog;
using Infrastructure.HumandClient;

namespace AMPM_CentralHubAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EventLogController : ControllerBase {
        private readonly HumandEmployeeRepository _humandEmployeeRepository;
        private readonly AmPmCentralHubContext _dbContext;

        public EventLogController(HumandEmployeeRepository humandEmployeeRepository, AmPmCentralHubContext dbContext) {
            _humandEmployeeRepository = humandEmployeeRepository;
            _dbContext = dbContext;
        }

        [HttpPost("add-EventLog")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> addEventLog(CreateEventLogRequest eventLogRequest)
        {
            if (eventLogRequest == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { message= "Event not set" });
            if (_dbContext.Database.CanConnect() == false)
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
            try
            {
                // Map CreateEventLogRequest to EventLog
                var eventLog = new EventLog
                {
                    Data = eventLogRequest.Data ?? string.Empty,
                    EventType = eventLogRequest.EventType ?? string.Empty,
                    CreatedAt = eventLogRequest.CreatedAt == default ? DateTime.Now : eventLogRequest.CreatedAt,
                    EmployeeIdentification = eventLogRequest.EmployeeIdentification ?? string.Empty
                };
                if (eventLogRequest.IdStoreHQ.Contains("AMPM"))
                    eventLogRequest.IdStoreHQ = eventLogRequest.IdStoreHQ.Substring(4, 2);
                // Parse IdStoreHQ which comes as string in the request
                if (!string.IsNullOrWhiteSpace(eventLogRequest.IdStoreHQ) &&
                    int.TryParse(eventLogRequest.IdStoreHQ, out var idStore))
                {
                    eventLog.IdStoreHQ = idStore.ToString();
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { messsage = "IdStore of Event is not defined" });
                }

                await _dbContext.EventLogs.AddAsync(eventLog);
                await _dbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new { value = eventLog });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("add-EventLog-ClockIn")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> addEventLogClockIn(CreateEventLogRequest eventLogRequest)
        {
            if (eventLogRequest == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Event not set" });
            if (_dbContext.Database.CanConnect() == false)
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
            try
            {
                // Map CreateEventLogRequest to EventLog
                var eventLog = new EventLog
                {
                    Data = eventLogRequest.Data ?? string.Empty,
                    EventType = eventLogRequest.EventType ?? string.Empty,
                    CreatedAt = eventLogRequest.CreatedAt,
                    EmployeeIdentification = eventLogRequest.EmployeeIdentification ?? string.Empty
                };

                // Parse IdStoreHQ which comes as string in the request
                if (!string.IsNullOrWhiteSpace(eventLogRequest.IdStoreHQ) &&
                    int.TryParse(eventLogRequest.IdStoreHQ, out var idStore))
                {
                    eventLog.IdStoreHQ = idStore.ToString();
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { messsage = "IdStore of Event is not defined" });
                }

                await _dbContext.EventLogs.AddAsync(eventLog);
                await _dbContext.SaveChangesAsync();

                HumandTimeSerialize humandTime =    new HumandTimeSerialize
                {
                    employeeId = eventLog.EmployeeIdentification,
                    now = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                    comment = "Clock In from AMPM Central Hub"
                };
                
                var response = await _humandEmployeeRepository.clockIn(humandTime);

                if(response.id > 0)
                    return StatusCode(StatusCodes.Status200OK, new { value = eventLog });
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.type });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        [HttpPost("add-EventLog-ClockOut")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> addEventLogClockOut(CreateEventLogRequest eventLogRequest)
        {
            if (eventLogRequest == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Event not set" });
            if (_dbContext.Database.CanConnect() == false)
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
            try
            {
                // Map CreateEventLogRequest to EventLog
                var eventLog = new EventLog
                {
                    Data = eventLogRequest.Data ?? string.Empty,
                    EventType = eventLogRequest.EventType ?? string.Empty,
                    CreatedAt = eventLogRequest.CreatedAt,
                    EmployeeIdentification = eventLogRequest.EmployeeIdentification ?? string.Empty
                };

                // Parse IdStoreHQ which comes as string in the request
                if (!string.IsNullOrWhiteSpace(eventLogRequest.IdStoreHQ) &&
                    int.TryParse(eventLogRequest.IdStoreHQ, out var idStore))
                {
                    eventLog.IdStoreHQ = idStore.ToString();
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { messsage = "IdStore of Event is not defined" });
                }

                await _dbContext.EventLogs.AddAsync(eventLog);
                await _dbContext.SaveChangesAsync();
                HumandTimeSerialize humandTime = new HumandTimeSerialize
                {
                    employeeId = eventLog.EmployeeIdentification,
                    now = eventLog.CreatedAt.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                    comment = "Clock Out from AMPM Central Hub"
                };
                var response = await _humandEmployeeRepository.clockOut(humandTime);
                if (response.id > 0)
                    return StatusCode(StatusCodes.Status200OK, new { value = eventLog });
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.type });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

    }
}
