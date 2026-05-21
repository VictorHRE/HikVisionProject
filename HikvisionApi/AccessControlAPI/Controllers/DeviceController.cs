using Application.Devices.DeviceDtos;
using Application.Devices.GetHostInfo;
using Application.Devices.GetHttpListener;
using Application.Devices.SetHttpListener;
using Application.Response;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Bus;
using Shared.Domain.Query;
using System.Net.NetworkInformation;
using System.Xml.Linq;

using HttpListenerConfig = AMPMAccesControlAPI.Request.HttpListener.HttpListenerRequest;

namespace AMPMAccessControlAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class DeviceController : ControllerBase
{
	private readonly IQueryBus _queryBus;
	private readonly ICommandBus _commandBus;
	private readonly IConfiguration _configuration;

	private readonly string _ipAddress;

	public DeviceController(ICommandBus commandBus, IQueryBus queryBus, IConfiguration configuration)
	{
		_commandBus = commandBus;
		_queryBus = queryBus;
		_configuration = configuration;
		_ipAddress = _configuration["HikVisionDevice:Host"] ?? throw new Exception("No se pudo obtener la ip de appsettings");

		_ipAddress = _ipAddress.Replace("http://", "").Replace("https://", "").Split(':')[0];
	}


	[HttpPost("connect")]
	public async Task<ActionResult> ConnectDevice()
	{
		using var ping = new Ping();

		try
		{
			PingReply reply = await ping.SendPingAsync(_ipAddress, 3000); // 3000ms timeout

			if (reply.Status == IPStatus.Success)
			{
				return Ok($"Ping a {_ipAddress} exitoso: {reply.RoundtripTime} ms");
			}
			else
			{
				return BadRequest($"Ping fallido: {reply.Status}");
			}
		}
		catch (Exception ex)
		{
			return BadRequest($"Error al hacer ping: {ex.Message}");
		}
	}

	[HttpGet("host-info")]
	public async Task<ActionResult> GetHostInfo()
	{
		var response = await _queryBus.AskAsync<XDocument>(new GetHostInfoQuery());

		var root = response.Root; // "DeviceInfo"
								  // Declarar el namespace (extraído del atributo xmlns)
		XNamespace ns = "http://www.isapi.org/ver20/XMLSchema";

		var deviceName = root?.Element(ns + "deviceName")?.Value;
		var model = root?.Element(ns + "model")?.Value;
		var serial = root?.Element(ns + "serialNumber")?.Value;
		var firmwareVersion = root?.Element(ns + "firmwareVersion")?.Value;
		var releaseDate = root?.Element(ns + "firmwareReleasedDate")?.Value;
		var mac = root?.Element(ns + "macAddress")?.Value;
		var deviceType = root?.Element(ns + "deviceType")?.Value;
		var manufacturer = root?.Element(ns + "manufacturer")?.Value;

		return Ok(new
		{
			deviceName,
			serial,
			model,
			firmwareVersion
		});
	}

	[HttpGet("http-listener")]
	public async Task<ActionResult> GetHttpListener()
	{
		var response = await _queryBus.AskAsync<ApiResponse<List<HttpListenerDto>>>(new GetHttpListenerQuery());


		if (!response.Success)
		{
			return StatusCode(response.StatusCode, response.Message);
		}

		return Ok(response.SerializableResponse());
	}

	[HttpPut("configure-http-listener")]
	public async Task<ActionResult> ConfigureHttpListener([FromBody] HttpListenerConfig httpListener)
	{
		try
		{
			var request = await _commandBus.DispatchAsync<ApiResponse<bool>>(new SetHttpListenerCommand()
			{
				IpAddress = httpListener.IpAddress,
				Port = httpListener.Port,
				Protocol = httpListener.Protocol,
				Url = httpListener.Url
			});

			if (!request.Success)
			{
				return StatusCode(request.StatusCode, request.Message);
			}

			return Ok(request.SerializableResponse());
		}
		catch (Exception ex)
		{
			return BadRequest($"Error al configurar el HTTP Listener: {ex.Message}");
		}
	}
}
