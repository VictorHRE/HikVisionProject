namespace Infrastructure.HttpClients.HumandClient;

public class HumandUserSerialize
{
	public int count { get; set; }

	public List<UserResponse> users { get; set; }
}

public class UserResponse
{
	public int id { get; set; }

	public string birthdate { get; set; }

	public string email { get; set; }
	public string phoneNumber { get; set; }

	public string employeeInternalId { get; set; }

	public string firstName { get; set; }

	public string lastName { get; set; }

	public string status { get; set; }
}
