using Application.Devices.DeviceDtos;
using Infrastructure.UnitOfWork;
using Shared.Domain.Query;

namespace Application.Devices.GetDevice;

public class GetDeviceQueryHandler : IQueryHandler<GetDeviceQuery, GetDeviceDto?>
{
	private readonly IUnitOfWork _unitOfWork;

	public GetDeviceQueryHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<GetDeviceDto?> Handle(GetDeviceQuery query)
	{
		var device = await _unitOfWork.DeviceRepository.GetDeviceAsync(new Domain.Device.DeviceId(query.Id));

		if (device is null)
		{
			return null;
		}

		return new GetDeviceDto(
			device.Id.Id,
			device.Name,
			device.IpAddress,
			device.DeviceMacAddress.MacAddress,
			device.Username,
			device.Password,
			device.Port,
			device.Status.ToString(),
			device.CreatedAt,
			device.UpdatedAt
		);
	}
}
