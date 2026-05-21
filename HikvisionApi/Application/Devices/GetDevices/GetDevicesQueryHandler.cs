using Application.Devices.DeviceDtos;
using Domain.Device;
using Infrastructure.UnitOfWork;
using Shared.Domain.Query;

namespace Application.Devices.GetDevices;

public class GetDevicesQueryHandler : IQueryHandler<GetDevicesQuery, IEnumerable<GetDeviceDto>>
{
	private readonly IUnitOfWork _unitOfWork;

	public GetDevicesQueryHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}


	public async Task<IEnumerable<GetDeviceDto>> Handle(GetDevicesQuery query)
	{
		var devices = await _unitOfWork.DeviceRepository.GetDevicesAsync(query.Name, query.IpAddress);

		if (!devices.Any())
		{
			return [];
		}

		return devices.Select(device => new GetDeviceDto(
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
		));
	}
}
