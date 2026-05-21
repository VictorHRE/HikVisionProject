using Domain.Device;
using Infrastructure.UnitOfWork;
using Shared.Domain.Bus;

namespace Application.Devices.UpdateDevice;

/*public class UpdateDeviceCommandHandler : ICommandHandler<UpdateDeviceCommand>
{
	private readonly IUnitOfWork _unitOfWork;

	public UpdateDeviceCommandHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}


	public async Task Handle(UpdateDeviceCommand command)
	{

		try
		{
			var device = await _unitOfWork.DeviceRepository.GetDeviceAsync(new Domain.Device.DeviceId(command.Id));

			if (device is null) throw new Exception($"Device with id {command.Id} not found");

			var newDevice = new Device
			{
				Name = command.DeviceName,
				IpAddress = command.IpAddress,
				DeviceMacAddress = new DeviceMacAddress(command.DeviceMacAddress),
				Username = command.Username,
				Password = command.Password,
				Port = command.Port,
				Status = Enum.Parse<DeviceStatus>(command.Status),
			};

			device.Update(newDevice);

			await _unitOfWork.DeviceRepository.UpdateDeviceAsync(device, newDevice, CancellationToken.None);

		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}

	}
}*/
