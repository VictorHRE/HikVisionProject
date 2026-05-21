using Domain.Employee;

namespace Domain.Attendance;

public class EmployeeAttendance
{
	public int Id { get; set; }

	public EmployeeId EmployeeNumber { get; set; }

	public AttendanceType AttendanceType { get; set; }

	public DateTime Time { get; set; }

	public EmployeeAttendance(EmployeeId employeeId, DateTime time, AttendanceType attendanceType)
	{
		EmployeeNumber = employeeId;
		Time = time;
		AttendanceType = attendanceType;
	}

	public EmployeeAttendance()
	{
		Id = 0;
		EmployeeNumber = new EmployeeId(string.Empty);
		Time = DateTime.Now;
		AttendanceType = AttendanceType.Unknown;
	}
}
