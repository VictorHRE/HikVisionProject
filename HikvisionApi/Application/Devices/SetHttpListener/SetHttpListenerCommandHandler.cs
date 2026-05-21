using Application.Response;
using Domain.Device;
using Domain.Host;
using Shared.Domain.Bus;
using Shared.Helpers;

namespace Application.Devices.SetHttpListener;

public class SetHttpListenerCommandHandler : ICommandHandler<SetHttpListenerCommand, ApiResponse<bool>>
{
	private readonly IDeviceRepository _deviceRepository;

	public SetHttpListenerCommandHandler(IDeviceRepository deviceRepository)
	{
		_deviceRepository = deviceRepository;
	}

	public async Task<ApiResponse<bool>> Handle(SetHttpListenerCommand command)
	{

		try
		{
			if (!command.Protocol.Equals("http", StringComparison.InvariantCultureIgnoreCase)
			&& !command.Protocol.Equals("https", StringComparison.CurrentCultureIgnoreCase))
			{
				throw new Exception("Invalid protocol");
			}

			if (!command.IpAddress.IsValidIp())
			{
				throw new Exception("Invalid IP address");
			}

			if (command.Port < 1 || command.Port > 65535)
			{
				throw new Exception("Invalid port");
			}

			var httpListener = new HttpListener()
			{
				Id = 1,
				IpAddress = command.IpAddress,
				Port = command.Port,
				Url = command.Url,
				Protocol = command.Protocol.ToUpper()
			};

			await _deviceRepository.SetHttpListenerAsync(httpListener);

			return new ApiResponse<bool>()
			{
				Data = true,
				Message = "HTTP listener set successfully",
				Success = true,
				StatusCode = 200
			};
		}
		catch (Exception ex)
		{
			return new ApiResponse<bool>()
			{
				Data = false,
				Message = $"Error setting HTTP listener: {ex.Message}",
				Success = false,
				StatusCode = 500
			};
		}
	}
}
