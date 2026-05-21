using Domain.Device;
using Domain.EventLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
{
	public void Configure(EntityTypeBuilder<EventLog> builder)
	{
		builder.ToTable("eventLogs");

		builder.HasKey(x => new
		{
			x.Id,
		});

		builder.Property(x => x.Id)
			.ValueGeneratedOnAdd();

		builder.Property(x => x.Data)
			.IsRequired(false)
			.HasColumnName("data");

		builder.Property(x => x.EventType)
			.IsRequired(false)
			.HasColumnName("eventType");
		
		builder.Property(x => x.IdStoreHQ)
			.IsRequired(true)
			.HasColumnName("IdStoreHQ").HasDefaultValue(0);
		
		builder.Property(x => x.EmployeeIdentification)
			.IsRequired(true)
			.HasColumnName("EmployeeIdentification")
			.HasDefaultValue("-");

		builder.Property(x => x.CreatedAt)
			.HasColumnName("createdAt");
	}
}
