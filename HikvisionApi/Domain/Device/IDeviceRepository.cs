using Domain.Host;
using System.Xml.Linq;

namespace Domain.Device;


public interface IDeviceRepository
{
	Task<Device?> GetDeviceAsync(DeviceId id);

	Task<XDocument> GetHostInfoAsync();

	Task<IEnumerable<Device>> GetDevicesAsync(string? name = "", string? ipAddress = "");

	Task<int> AddDeviceAsync(Device device, CancellationToken cancellationToken);

	Task UpdateDeviceAsync(Device currentDevice, Device device, CancellationToken cancellationToken);

	Task DeleteDeviceAsync(DeviceId id, CancellationToken cancellationToken);

	Task InsertEventLogAsync(Domain.EventLog.EventLog eventLog, CancellationToken cancellationToken);

	Task InsertEventLogAsync(string data, int eventType, string idStore,CancellationToken cancellationToken);

	Task SetHttpListenerAsync(HttpListener listener);

	Task<List<HttpListener>> GetHttpListenerAsync();
}