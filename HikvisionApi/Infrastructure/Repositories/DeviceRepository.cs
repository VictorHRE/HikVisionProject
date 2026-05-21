using Domain.Device;
using Domain.Employee;
using Domain.EventLog;
using Domain.Host;
using Infrastructure.HttpClients.DigestClient;
using Infrastructure.ISAPI;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using System.Text.Json;
using System.Xml.Linq;

namespace Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
	private readonly AmpmAccessControlContext _context;
	private readonly DigestAuthClient _digestAuthClient;

	public DeviceRepository(AmpmAccessControlContext context, DigestAuthClient digestAuthClient)
	{
		_context = context;
		_digestAuthClient = digestAuthClient;
	}

	public async Task<XDocument> GetHostInfoAsync()
	{
		var response = await _digestAuthClient.GetDigestAuthAsync("ISAPI/System/deviceInfo");
		var xml = await response.Content.ReadAsStreamAsync();

		var xmlString = new StreamReader(xml).ReadToEnd();

		var doc = XDocument.Parse(xmlString);

		return doc;
	}

	public async Task<int> AddDeviceAsync(Device device, CancellationToken cancellationToken)
	{
		var result = await _context.Devices.AddAsync(device, cancellationToken);

		return result.Entity.Id.Id;
		//await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteDeviceAsync(DeviceId id, CancellationToken cancellationToken)
	{
		var device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
			?? throw new Exception($"Device with id {id} not found");

		_context.Devices.Remove(device);
	}

	public async Task<Device?> GetDeviceAsync(DeviceId id)
	{
		var device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id);

		return device;
	}

	public async Task<IEnumerable<Device>> GetDevicesAsync(string? name = "", string? ipAddress = "")
	{
		var devices = _context.Devices.AsQueryable();

		if (!string.IsNullOrEmpty(name))
			devices = devices.Where(x => x.Name.Contains(name, StringComparison.InvariantCulture));

		if (!string.IsNullOrEmpty(ipAddress))
			devices = devices.Where(x => x.IpAddress.Contains(ipAddress, StringComparison.InvariantCulture));

		return await devices.ToListAsync();
	}

	public async Task InsertEventLogAsync(EventLog eventLog, CancellationToken cancellationToken)
	{
		await _context.EventLogs.AddAsync(eventLog, cancellationToken);
	}

	public async Task InsertEventLogAsync(string data, int eventType, string idStore, CancellationToken cancellationToken)
	{

		try
		{
			var subEventType = (SubEventType)eventType;

			var eventLog = new EventLog(data, DateTime.UtcNow, subEventType.ToString(), idStore, "-");

			await InsertEventLogAsync(eventLog, cancellationToken);
		}
		catch (Exception exception)
		{
			throw new Exception(exception.Message, exception);
		}
	}

	public async Task SetHttpListenerAsync(HttpListener listener)
	{
		var bodyXml = @$"<?xml version=""1.0"" encoding=""UTF-8""?>
						<HttpHostNotificationList version=""2.0"" xmlns=""http://www.isapi.org/ver20/XMLSchema"">

						  <HttpHostNotification>
							<!-- Requeridos -->
							<id>{listener.Id}</id>
							<url>{listener.Url}</url>
							<protocolType>{listener.Protocol}</protocolType>
							<parameterFormatType>JSON</parameterFormatType>
							<addressingFormatType>ipaddress</addressingFormatType>
							<httpAuthenticationMethod>none</httpAuthenticationMethod>

							<ipAddress>{listener.IpAddress}</ipAddress>
							<portNo>{listener.Port}</portNo>
							<!-- uploadImagesDataType: ""binary"" (default) o ""URL"" -->
							<uploadImagesDataType>binary</uploadImagesDataType>
						  </HttpHostNotification>

						</HttpHostNotificationList>";

		var response = await _digestAuthClient.PutDigestAuthAsync(Routes.SetHost, bodyXml, "application/xml");

		var responseContent = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
		{
			throw new Exception($"Error setting HTTP listener: {response.StatusCode} - {responseContent}");
		}

		var result = XmlHelper<ISAPIXMLHostResponseStatus>.DeserializeResponseStatus(responseContent);

		if (result.StatusString != "OK")
		{
			throw new Exception($"Error setting HTTP listener: {result.StatusString} - {responseContent}");
		}
	}

	public async Task<List<HttpListener>> GetHttpListenerAsync()
	{
		var response = await _digestAuthClient.GetDigestAuthAsync(Routes.ListHosts);

		var responseContent = await response.Content.ReadAsStringAsync();

		var result = XmlHelper<HttpHostNotificationList>.DeserializeResponseStatus(responseContent);

		if (response.IsSuccessStatusCode)
		{
			return [.. result.Notifications.Select(x => new HttpListener()
			{
				Id = int.Parse(x.Id),
				Url = x.Url,
				Port = x.PortNo,
				Protocol = x.ProtocolType,
				IpAddress = x.IpAddress
			})];
		}

		return [];
	}

	public Task UpdateDeviceAsync(Device currentDevice, Device device, CancellationToken cancellationToken)
	{
		//currentDevice.Update(device);

		_context.Devices.Update(currentDevice);

		return Task.CompletedTask;
	}
}