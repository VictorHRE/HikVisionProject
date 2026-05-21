using Domain.Attendance;
using Domain.Device;
using Domain.Employee;
using Domain.EventLog;
using Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AmpmAccessControlContext(DbContextOptions<AmpmAccessControlContext> options) : DbContext(options)
{

	public DbSet<Device> Devices { get; set; }

	public DbSet<EventLog> EventLogs { get; set; }

	public DbSet<Employee> Employees { get; set; }

	public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new DeviceConfiguration());
		modelBuilder.ApplyConfiguration(new EventLogConfiguration());
		modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
		modelBuilder.ApplyConfiguration(new EmployeeAttendanceConfiguration());

		base.OnModelCreating(modelBuilder);
	}
}