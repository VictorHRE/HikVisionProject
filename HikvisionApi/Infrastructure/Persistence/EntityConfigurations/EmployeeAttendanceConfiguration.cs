using Domain.Attendance;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence.EntityConfigurations;

public class EmployeeAttendanceConfiguration : IEntityTypeConfiguration<EmployeeAttendance>
{
	public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<EmployeeAttendance> builder)
	{
		builder.ToTable("employeeAttendances");

		builder.HasKey(x => new
		{
			x.Id,
			x.EmployeeNumber,
		});

		builder.Property(x => x.Id)
			.HasColumnName("Id")
			.ValueGeneratedOnAdd();

		builder.Property(x => x.EmployeeNumber)
			.HasColumnName("employeeNumber")
			.HasConversion(x => x.Value, x => new Domain.Employee.EmployeeId(x))
			.IsRequired();

		builder.Property(x => x.AttendanceType)
			.HasColumnName("attendanceType")
			.HasConversion(x => x.ToString(), x => (AttendanceType)Enum.Parse(typeof(AttendanceType), x))
			.IsRequired();

		builder.Property(x => x.Time)
			.HasColumnName("time")
			.IsRequired();

	}
}
