using Shared.Domain.Query;

namespace Application.Devices.GetDevice;

public class GetDeviceQuery : Query
{
	public int Id { get; private set; }

	public GetDeviceQuery(int id)
	{ 
		Id = id; 
	}
}
