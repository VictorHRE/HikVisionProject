using Application.Response;
using Domain.Device;
using Infrastructure.UnitOfWork;
using Shared.Domain.Bus;
using Shared.Helpers;

namespace Application.Devices.CreateDevice;

/// <summary>
/// CreateDeviceCommandHandler
/// </summary>
public class CreateDeviceCommandHandler : ICommandHandler<CreateDeviceCommand, ApiResponse<int>>
{
	private readonly IUnitOfWork _unitOfWork;

	public CreateDeviceCommandHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<ApiResponse<int>> Handle(CreateDeviceCommand command)
	{
		try
		{
			var status = Enum.TryParse<DeviceStatus>(command.Status, ignoreCase: true, out var statusResult)
				? statusResult
				: DeviceStatus.UNKNOWN;

			if (command.IpAddress.IsValidIp())
			{
				throw new Exception("Invalid IpAddress");
			}

			var device = Device.Create(
			command.DeviceName,
			command.IpAddress,
			new DeviceMacAddress(command.DeviceMacAddress),
			command.Username,
			command.Password,
			status,
			command.Port);

			var result = await _unitOfWork.DeviceRepository.AddDeviceAsync(device, CancellationToken.None);

			await _unitOfWork.CommitAsync(CancellationToken.None);

			return new ApiResponse<int>(message: "Device created", success: true, data: result, statusCode: 200);
		}
		catch (Exception ex)
		{
			return new ApiResponse<int>(message: ex.Message, success: false, data: 0, statusCode: 500);
		}
	}
}
