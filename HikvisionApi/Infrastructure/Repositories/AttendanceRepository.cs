using Domain.Attendance;
using Domain.Employee;
using Infrastructure.HttpClients.CentralHubClient;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public class AttendanceRepository(
	AmpmAccessControlContext ampmAccessControlContext,
	ICentralApiHttpClient centralApiHttpClient,
	IConfiguration configuration
	) : IAttendanceRepository
{
	private readonly AmpmAccessControlContext _ampmAccessControlContext = ampmAccessControlContext;
	private readonly string idStoreHQ = configuration["CentralHubCfg:IdStore"]
		?? throw new Exception("IdStore not found in configuration");


	public async Task<List<EmployeeAttendance>> GetDayEmployeeAttendancesAsync(Employee employee, AttendanceType attendanceType)
	{
		return await _ampmAccessControlContext
		   .EmployeeAttendances
		   .Where(x => x.EmployeeNumber == employee.Identification)
		   .Where(x => x.AttendanceType == attendanceType)
		   .Where(x => x.Time >= DateTime.Today && x.Time < DateTime.Today.AddDays(1))
		   .ToListAsync();
	}

	public async Task<EmployeeAttendance?> GetEmployeeAttendanceAsync(Employee employee)
	{
		return await _ampmAccessControlContext
		   .EmployeeAttendances
		   .Where(x => x.EmployeeNumber == employee.Identification)
		   //.Where(x => x.AttendanceType == AttendanceType.CheckIn)
		   .Where(x => x.Time >= DateTime.Today && x.Time < DateTime.Today.AddDays(1))
		   .FirstOrDefaultAsync();

		var checkOutAttendance = await _ampmAccessControlContext
		   .EmployeeAttendances
		   .Where(x => x.EmployeeNumber == employee.Identification)
		   .Where(x => x.AttendanceType == AttendanceType.CheckOut)
		   .Where(x => x.Time >= DateTime.Today && x.Time < DateTime.Today.AddDays(1))
		   .FirstOrDefaultAsync();
	}

	public async Task InsertAttendanceAsync(Employee employee, AttendanceType attendanceType)
	{
		try
		{
			await _ampmAccessControlContext.AddAsync(new EmployeeAttendance(
					employeeId: employee.Identification,
					attendanceType: attendanceType,
					time: DateTime.Now
				));

			await _ampmAccessControlContext.SaveChangesAsync();

			return;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}
}