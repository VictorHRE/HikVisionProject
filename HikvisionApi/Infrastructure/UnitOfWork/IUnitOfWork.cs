using Domain.Attendance;
using Domain.Device;
using Domain.Employee;

namespace Infrastructure.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
	IDeviceRepository DeviceRepository { get; }

	IEmployeeRepository EmployeeRepository { get; }
	
	IAttendanceRepository  AttendanceRepository { get; }

	Task<int> CommitAsync(CancellationToken cancellationToken);
}