using Domain.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
	public void Configure(EntityTypeBuilder<Employee> builder)
	{
		builder.ToTable("employees");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedOnAdd();

		builder.Property(x => x.Identification)
			.HasConversion(x => x.Value, x => new EmployeeId(x))
			.IsRequired()
			.HasColumnName("identification");

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(x => x.LastName)
			.IsRequired()
			.HasMaxLength(200)
			.HasColumnName("lastName");

		builder.Property(x => x.Email)
			.IsRequired(false)
			.HasMaxLength(200)
			.HasColumnName("email");

		builder.Property(x => x.Phone)
			.IsRequired()
			.HasMaxLength(20)
			.HasColumnName("phone");

		builder.Property(x => x.Position)
			.IsRequired(false)
			.HasMaxLength(100)
			.HasColumnName("position");

		builder.Property(x => x.BranchId)
			.IsRequired()
			.HasColumnName("branchId");

		builder.Property(x => x.Status)
			.IsRequired()
			.HasConversion(x => x.ToString(), x => (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), x))
			.HasColumnName("status");

		builder.Property(x => x.UserType)
			.IsRequired(false)
			.HasConversion(x => x.ToString(), x => (EmployeeType)Enum.Parse(typeof(EmployeeType), x))
			.HasColumnName("userType");

		builder.Property(x => x.Gender)
			.IsRequired(false)
			.HasConversion(x => x.ToString(), x => (EmployeeGender)Enum.Parse(typeof(EmployeeGender), x))
			.HasColumnName("gender");

		builder.Property(x => x.BeginTime)
			.IsRequired()
			.HasColumnName("beginTime");

		builder.Property(x => x.EndTime)
			.IsRequired()
			.HasColumnName("endTime");

		builder.Property(x => x.BirthDate)
			.IsRequired()
			.HasColumnName("birthDate");

		builder.Property(x => x.CreatedAt)
			.HasColumnName("createdAt");

		builder.HasIndex(x => x.Identification)
			.IsUnique();
	}
}