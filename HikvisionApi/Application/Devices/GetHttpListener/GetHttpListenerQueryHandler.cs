using Application.Devices.DeviceDtos;
using Application.Response;
using Domain.Device;
using Shared.Domain.Query;

namespace Application.Devices.GetHttpListener;

public class GetHttpListenerQueryHandler : IQueryHandler<GetHttpListenerQuery, ApiResponse<List<HttpListenerDto>>>
{
	private readonly IDeviceRepository _deviceRepository;

	public GetHttpListenerQueryHandler(IDeviceRepository deviceRepository)
	{
		_deviceRepository = deviceRepository;
	}

	public async Task<ApiResponse<List<HttpListenerDto>>> Handle(GetHttpListenerQuery query)
	{
		var httpListeners = await _deviceRepository.GetHttpListenerAsync();

		if (httpListeners.Count > 0)
		{
			var httpListenerDtos = httpListeners.Select(x => new HttpListenerDto()
			{
				Id = x.Id,
				Url = x.Url,
				Port = x.Port,
				Protocol = x.Protocol,
				IpAddress = x.IpAddress
			}).ToList();


			return new ApiResponse<List<HttpListenerDto>>()
			{
				Data = httpListenerDtos,
				Success = true
			};
		}

		return new ApiResponse<List<HttpListenerDto>>()
		{
			Data = new(),
			Success = true
		};
	}
}
