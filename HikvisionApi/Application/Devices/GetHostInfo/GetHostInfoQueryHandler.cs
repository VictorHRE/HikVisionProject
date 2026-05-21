

using Domain.Device;
using Shared.Domain.Query;
using System.Xml.Linq;

namespace Application.Devices.GetHostInfo;
public class GetHostInfoQueryHandler	: IQueryHandler<GetHostInfoQuery, XDocument>
{
	private readonly IDeviceRepository _deviceRepository;

	public GetHostInfoQueryHandler(IDeviceRepository deviceRepository)
	{
		_deviceRepository = deviceRepository;
	}

	public async Task<XDocument> Handle(GetHostInfoQuery query)
	{
		var hostInfo = await _deviceRepository.GetHostInfoAsync();

		return hostInfo;
	}
}
