using AMPM_CentralHubAPI.Request.Employee;
using Application.Response;
using Domain.Usuario;
using Domain.Employee;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.HttpClients.AccessControlClient;
using Infrastructure.HttpClients.HumandClient;
using Domain.ConTienda;

namespace AMPM_CentralHubAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase {
        private readonly HumandEmployeeRepository _humandEmployeeRepository;
        private readonly AccessControlRepository _accessControlRepository;
        private readonly AmPmCentralHubContext _dbContext;
        public EmployeeController(HumandEmployeeRepository humandEmployeeRepository,
            AccessControlRepository accessControlRepository,
            AmPmCentralHubContext dbContext)
        {
            _humandEmployeeRepository = humandEmployeeRepository;
            _dbContext = dbContext;
            _accessControlRepository = accessControlRepository;
        }

        [HttpPost("get-employee-by-id")]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetEmployeeById([FromBody] string employeeNo)
        {
            try
            {
                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
                var employee = _dbContext.EmployeeHubs
                    .FirstOrDefault(e => e.Identification == employeeNo);
                if (employee == null)
                {
                    var allUsers = new List<UserResponse>();
                    int totalCount = 0;
                    int pageNumber = 1;
                    const int pageSize = 50;

                    while (true)
                    {
                        var pageResult = await _humandEmployeeRepository.GetCustomers(pageNumber, pageSize);

                        if (pageResult == null)
                            break;

                        if (pageResult.users != null && pageResult.users.Count > 0)
                            allUsers.AddRange(pageResult.users);

                        // set total count from response
                        totalCount = pageResult.count;
                        var employeeRes = allUsers.Where(u =>
                            (!string.IsNullOrWhiteSpace(u.employeeInternalId) && u.employeeInternalId == employeeNo) ||
                            (u.id.ToString() == employeeNo)
                        ).FirstOrDefault();
                        if (employeeRes != null && !string.IsNullOrWhiteSpace(employeeRes.employeeInternalId))
                        {
                            DateTime birthDate = DateTime.MinValue;
                            if (!string.IsNullOrWhiteSpace(employeeRes.birthdate))
                            {
                                DateTime.TryParse(employeeRes.birthdate, out birthDate);
                            }
                            var newEmp = new EmployeeHub
                            {
                                Identification = employeeRes.employeeInternalId,
                                Name = employeeRes.firstName ?? string.Empty,
                                LastName = employeeRes.lastName ?? string.Empty,
                                Position = string.Empty,
                                Phone = employeeRes.phoneNumber,
                                Email = employeeRes.email,
                                IdStoreHQ = null,
                                Status = employeeRes.status ?? string.Empty,
                                UserType = string.Empty,
                                Gender = string.Empty,
                                BeginTime = DateTime.Now,
                                EndTime = DateTime.Now.AddYears(10),
                                BirthDate = birthDate == DateTime.MinValue ? DateTime.Now : birthDate,
                                CreatedAt = DateTime.Now
                            };
                            await _dbContext.EmployeeHubs.AddAsync(newEmp);
                            employee = newEmp;
                        }
                        else
                        {
                            // if total count known and we've retrieved all, stop
                            if (totalCount > 0 && allUsers.Count >= totalCount)
                                break;
                            // if returned fewer items than requested, no more pages
                            if (pageResult.users == null || pageResult.users.Count < pageSize)
                                break;
                            pageNumber++;
                        }

                    }
                }
                return Ok(new { value = employee });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("get-employees")]
        public async Task<IActionResult> getEmployees()
        {
            try
            {
                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
                var employees = _dbContext.EmployeeHubs.ToList();
                if (employees != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { value = employees });
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent, new { value = employees });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = $"Error en DB: {ex.Message}" });
            }

        }

        [HttpPost("get-employeesHumand")]
        public async Task<ActionResult> GetEmployeesHumand()
        {
            try
            {
                var allUsersResult = await getAllEmployees();

                // map and persist to database
                if (allUsersResult?.users != null && allUsersResult.users.Count > 0)
                {
                    foreach (var u in allUsersResult.users)
                    {
                        // choose identification from employeeInternalId if available otherwise id
                        var identification = !string.IsNullOrWhiteSpace(u.employeeInternalId) ? u.employeeInternalId : u.id.ToString();

                        var existing = await _dbContext.EmployeeHubs
                            .FirstOrDefaultAsync(e => e.Identification == identification);

                        DateTime birthDate = DateTime.MinValue;
                        if (!string.IsNullOrWhiteSpace(u.birthdate))
                        {
                            DateTime.TryParse(u.birthdate, out birthDate);
                        }

                        if (existing == null)
                        {
                            var newEmp = new EmployeeHub
                            {
                                Identification = identification,
                                Name = u.firstName ?? string.Empty,
                                LastName = u.lastName ?? string.Empty,
                                Position = string.Empty,
                                Phone = u.phoneNumber,
                                Email = u.email,
                                IdStoreHQ = null,
                                Status = u.status ?? string.Empty,
                                UserType = null,
                                Gender = null,
                                BeginTime = DateTime.Now,
                                EndTime = DateTime.Now.AddYears(10),
                                BirthDate = birthDate == DateTime.MinValue ? DateTime.Now : birthDate,
                                CreatedAt = DateTime.Now
                            };

                            await _dbContext.EmployeeHubs.AddAsync(newEmp);
                        }
                        else
                        {
                            existing.Name = u.firstName ?? existing.Name;
                            existing.LastName = u.lastName ?? existing.LastName;
                            existing.Email = u.email ?? existing.Email;
                            existing.Status = u.status ?? existing.Status;

                            if (birthDate != DateTime.MinValue)
                                existing.BirthDate = birthDate;

                            existing.BeginTime = existing.BeginTime == default ? DateTime.Now : existing.BeginTime;
                            existing.EndTime = existing.EndTime == default ? DateTime.Now.AddYears(10) : existing.EndTime;

                            _dbContext.EmployeeHubs.Update(existing);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }

                return Ok(new { value = allUsersResult });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("add-employeeToDevice")]
        public async Task<ActionResult> addEmployee(EmployeeHub employee)
        {
            try
            {
                if (employee == null || string.IsNullOrEmpty(employee.Identification)) {

                    //employee = _dbContext.EmployeeHubs.Where(e => e.Identification.Equals("ana.corea@ampm.com.ni")).FirstOrDefault();
                    //employee.IdStoreHQ = 1;
                    return BadRequest(new { isSuccess = false , message = "Los datos del empleado son requeridos" });
                }

                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
                var employeeSerialize = new EmployeeSerialize
                {
                    identificationNumber = employee.Identification,
                    Name = employee.Name,
                    LastName = employee.LastName,
                    Position = employee.Position,
                    Phone = employee.Phone,
                    Email = employee.Email,
                    BranchId = employee.IdStoreHQ ?? 0,
                    Status = employee.Status,
                    Gender = employee.Gender ?? string.Empty,
                    BeginDate = employee.BeginTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndDate = employee.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    BirthDate = employee.BirthDate.ToString("yyyy-MM-ddTHH:mm:ss")
				};
                if (employee.IdStoreHQ > 0) {
                    ConTienda tienda = _dbContext.ConTiendas.Where(t => t.IdStoreHQ == employee.IdStoreHQ).FirstOrDefault();
                    string baseUrl = $"http://{tienda.Ip}:{tienda.Port}/api/";
                    bool res = _accessControlRepository.changeBaseUrl(baseUrl);
                }else {
                    return BadRequest(new { message = "La tienda es requerida para el empleado" });
                }
               
                var result = await _accessControlRepository.addEmployee(employeeSerialize);
                if (result.StatusCode == 200) {
                    _dbContext.EmployeeHubs.Update(employee);
                    await _dbContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { value = result , message="Empleado Agregado correctamente"});
                }   
                else
                    return StatusCode(StatusCodes.Status400BadRequest, new { value = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        // Additional methods (add/update) omitted for brevity

        [HttpPost("update-employeeToDevice")]
        public async Task<ActionResult> updateEmployee(EmployeeHub employee)
        {
            try
            {
                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
                EmployeeHub employeeBefore = _dbContext.EmployeeHubs.Where(e => e.Identification.Equals(employee.Identification)).AsNoTracking().FirstOrDefault();
                if (employeeBefore == null)
                    return BadRequest(new { isSuccess = false, message = "Empleado no encontrado" });
                var employeeSerialize = new EmployeeSerialize
                {
                    identificationNumber = employee.Identification,
                    Name = employee.Name,
                    LastName = employee.LastName,
                    Position = employee.Position,
                    Phone = employee.Phone,
                    Email = employee.Email,
                    BranchId = employee.IdStoreHQ ?? 0,
                    Status = employee.Status,
                    Gender = employee.Gender ?? string.Empty,
                    BeginDate = employee.BeginTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndDate = employee.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    BirthDate = employee.BirthDate.ToString("yyyy-MM-ddTHH:mm:ss")
				};
                var result= new EmployeeResponseSerialize();
                //si es cambio de estatus y misma tienda
                if (!employeeBefore.Status.Equals(employee.Status) && employeeBefore.IdStoreHQ.Equals(employee.IdStoreHQ))
                {
                    if (employee.IdStoreHQ > 0)
                    {
                        ConTienda tienda = _dbContext.ConTiendas.Where(t => t.IdStoreHQ == employee.IdStoreHQ).FirstOrDefault();
                        string baseUrl = $"http://{tienda.Ip}:{tienda.Port}/api/";
                        bool res = _accessControlRepository.changeBaseUrl(baseUrl);
                    }
                    else
                    {
                        return BadRequest(new { message = "La tienda es requerida para el empleado" });
                    }
                    result = await _accessControlRepository.updateEmployee(employeeSerialize);
                }
                //si es cambio de tienda
                else if (!employeeBefore.IdStoreHQ.Equals(employee.IdStoreHQ))
                {
                    if (employeeBefore.IdStoreHQ > 0)
                    {
                        ConTienda tienda = _dbContext.ConTiendas.Where(t => t.IdStoreHQ == employeeBefore.IdStoreHQ).FirstOrDefault();
                        string baseUrl = $"http://{tienda.Ip}:{tienda.Port}/api/";
                        bool res = _accessControlRepository.changeBaseUrl(baseUrl);
                    }
                    else
                    {
                        return BadRequest(new { message = "La tienda es requerida para el empleado" });
                    }
                    employeeSerialize.Status = "INACTIVE";
                    result = await _accessControlRepository.updateEmployee(employeeSerialize);
                    //agregrar en la nueva tienda
                    if (employee.IdStoreHQ > 0)
                    {
                        ConTienda tienda = _dbContext.ConTiendas.Where(t => t.IdStoreHQ == employee.IdStoreHQ).FirstOrDefault();
                        string baseUrl = $"http://{tienda.Ip}:{tienda.Port}/api/";
                        bool res = _accessControlRepository.changeBaseUrl(baseUrl);
                    }
                    else
                    {
                        return BadRequest(new { message = "La tienda es requerida para el empleado" });
                    }
                    result = await _accessControlRepository.addEmployee(employeeSerialize);
                }
                else {
                    result.StatusCode = 200;
                    result.Message = "";
                }
                    
                    //employee.Id = Nullable;
                    _dbContext.EmployeeHubs.Update(employee);
                await _dbContext.SaveChangesAsync();
                if (string.IsNullOrEmpty(result.Message))
                    result.Message = "Empleado actualizado correctamente";
                if (result.StatusCode == 200)
                    return StatusCode(StatusCodes.Status200OK, new { value = result });
                else
                    return StatusCode(StatusCodes.Status400BadRequest, new { value = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("add-FingerPrintedEmployee")]
        public async Task<ActionResult> addFingerPrintedEmployee(EmployeeHub employee)
        {
            try
            {
                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
                if (employee.IdStoreHQ > 0)
                {
                    ConTienda tienda = _dbContext.ConTiendas.Where(t => t.IdStoreHQ == employee.IdStoreHQ).FirstOrDefault();
                    string baseUrl = $"http://{tienda.Ip}:{tienda.Port}/api/";
                    bool res = _accessControlRepository.changeBaseUrl(baseUrl);
                }
                else
                {
                    return BadRequest(new { isSuccess = false,  message = "La tienda es requerida para el empleado" });
                }
                var result = await _accessControlRepository.addFingerPrintedEmployee(employee.Identification);
                if (result.FingerNo > 0) {
                    employee.Finger = "ASOCIADO";
                    _dbContext.EmployeeHubs.Update(employee);
                    await _dbContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { value = result });
                }
                    
                else
                    return StatusCode(StatusCodes.Status400BadRequest, new { value = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete("delete-FingerPrintedEmployee")]
        public async Task<ActionResult> deleteFingerPrintedEmployee(EmployeeHub employee)
        {
            try
            {
                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });

                if (employee.IdStoreHQ > 0)
                {
                    ConTienda tienda = _dbContext.ConTiendas.Where(t => t.IdStoreHQ == employee.IdStoreHQ).FirstOrDefault();
                    string baseUrl = $"http://{tienda.Ip}:{tienda.Port}/api/";
                    bool res = _accessControlRepository.changeBaseUrl(baseUrl);
                }
                else
                {
                    return BadRequest(new { message = "La tienda es requerida para el empleado" });
                }
                var result = await _accessControlRepository.deleteFingerPrintedEmployee(employee.Identification);
                if (result.StatusCode == 200) {
                    employee.Finger = "NOASOCIADO";
                    _dbContext.EmployeeHubs.Update(employee);
                    await _dbContext.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { value = result });
                }
                    
                else
                    return StatusCode(StatusCodes.Status400BadRequest, new { value = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        private async Task<HumandUserSerialize> getAllEmployees()
        {
            var allUsers = new List<UserResponse>();
            int totalCount = 0;
            int pageNumber = 1;
            const int pageSize = 50;

            while (true)
            {
                var pageResult = await _humandEmployeeRepository.GetCustomers(pageNumber, pageSize);

                if (pageResult == null)
                    break;

                if (pageResult.users != null && pageResult.users.Count > 0)
                    allUsers.AddRange(pageResult.users);

                // set total count from response
                totalCount = pageResult.count;

                // if total count known and we've retrieved all, stop
                if (totalCount > 0 && allUsers.Count >= totalCount)
                    break;

                // if returned fewer items than requested, no more pages
                if (pageResult.users == null || pageResult.users.Count < pageSize)
                    break;

                pageNumber++;
            }

            return new HumandUserSerialize { count = totalCount, users = allUsers };
        }


    }
}
