using Domain.Device;
using Microsoft.EntityFrameworkCore;
using System;
namespace Infrastructure.Persistence.EntityConfigurations;


public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{

	public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Device> builder)
	{

		builder.ToTable("devices");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasConversion(x => x.Id, x => new DeviceId(x))
			.ValueGeneratedOnAdd()
			.HasColumnName("id");

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(200)
			.HasColumnName("name");

		builder.Property(x => x.IpAddress)
			.IsRequired()
			.HasMaxLength(15)
			.HasColumnName("ipAddress");

		builder.Property(x => x.DeviceMacAddress)
			.HasConversion(x => x.MacAddress, x => new DeviceMacAddress(x))
			.IsRequired()
			.HasColumnName("deviceMacAddress");

		builder.Property(x => x.Username)
			.IsRequired()
			.HasMaxLength(20)
			.HasColumnName("username");

		builder.Property(x => x.Password)
			.IsRequired()
			.HasMaxLength(200)
			.HasColumnName("password");


		builder.Property(x => x.Port)
			.IsRequired()
			.HasColumnName("port");

		builder.Property(x => x.Status)
			.IsRequired()
			.HasConversion(x => x.ToString(), x => (DeviceStatus)Enum.Parse(typeof(DeviceStatus), x))
			.HasColumnName("status");

		builder.Property(x => x.CreatedAt)
			.HasColumnName("createdAt");

		builder.Property(x => x.UpdatedAt)
			.HasColumnName("updatedAt");
	}
}

