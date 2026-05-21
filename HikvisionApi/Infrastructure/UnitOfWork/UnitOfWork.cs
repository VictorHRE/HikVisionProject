using Domain.Attendance;
using Domain.Device;
using Domain.Employee;
using Infrastructure.Persistence;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork;

public sealed class UnitOfWork(
    AmpmAccessControlContext context,
    IDeviceRepository deviceRepository,
    IEmployeeRepository employeeRepository,
    IAttendanceRepository attendanceRepository)
    : IUnitOfWork
{
    private bool _disposed = false;

    public IDeviceRepository DeviceRepository { get; } = deviceRepository;

    public IEmployeeRepository EmployeeRepository { get; } = employeeRepository;

    public IAttendanceRepository AttendanceRepository { get; } = attendanceRepository;

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await context.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}