namespace Domain.Attendance;

public interface IAttendanceRepository
{
    Task InsertAttendanceAsync(Employee.Employee employee, AttendanceType attendanceType);

	Task<List<EmployeeAttendance>> GetDayEmployeeAttendancesAsync(Employee.Employee employee, AttendanceType attendanceType);
	Task<EmployeeAttendance?> GetEmployeeAttendanceAsync(Employee.Employee employee);
}