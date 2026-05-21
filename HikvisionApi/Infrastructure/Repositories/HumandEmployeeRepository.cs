using Infrastructure.HttpClients.HumandClient;
using Domain.Employee;
using Infrastructure.HumandClient;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Infrastructure.Repositories;

public class HumandEmployeeRepository
{
    private readonly IHumanHttpClient _httpClient;

    public HumandEmployeeRepository(IHumanHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HumandUserSerialize> GetCustomers(int? page = 1, int? pageSize = 10)
    {
        var url = $"users?page={page}&limit={pageSize}";

        var response = await _httpClient.SendGetRequestAsync(url);

		var content = await response.Content.ReadAsStringAsync();

		if (response.IsSuccessStatusCode)
        {            
            var customers = Newtonsoft.Json.JsonConvert.DeserializeObject<HumandUserSerialize>(content);

            return customers!;
        }
        
        throw new Exception($"Failed to get customers: {content}");
    }
    public async Task<HumandTimeResponseSerialize> clockIn(HumandTimeSerialize timeSerialize)
    {
        var url = $"time-tracking/entries/clockIn";

        var body = JsonSerializer.Serialize(new
        {
			employeeId = timeSerialize.employeeId,
			now = DateTime.Now,
			comment = timeSerialize.comment,
		});

		var response = await _httpClient.SendPostRequestAsync(url, body);

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var responseJson = JObject.Parse(content);
            HumandTimeResponseSerialize humandTime = null;
            if (responseJson.ContainsKey("id"))
            {
                humandTime = new HumandTimeResponseSerialize
                {
                    id = (int)responseJson["id"],
                    userId = (int)responseJson["userId"],
                    employeeInternalId = (string)responseJson["employeeInternalId"],
                     type = (string)responseJson["type"]
                };
            }
            else {
                humandTime = new HumandTimeResponseSerialize
                {
                    id = 0,
                    type = responseJson["code"].ToString()+" : "+ responseJson["message"].ToString()
                };
            }

                return humandTime;
        }

        throw new Exception($"Failed to add clockIn EventLog: {content}");
    }

    public async Task<HumandTimeResponseSerialize> clockOut(HumandTimeSerialize timeSerialize)
    {
        var url = $"time-tracking/entries/clockOut";

        var response = await _httpClient.SendPostRequestAsync(url, JsonSerializer.Serialize(timeSerialize));

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var responseJson = JObject.Parse(content);
            HumandTimeResponseSerialize humandTime = null;
            if (responseJson.ContainsKey("id"))
            {
                humandTime = new HumandTimeResponseSerialize
                {
                    id = (int)responseJson["id"],
                    userId = (int)responseJson["userId"],
                    employeeInternalId = (string)responseJson["employeeInternalId"],
                    type = (string)responseJson["type"]
                };
            }
            else
            {
                humandTime = new HumandTimeResponseSerialize
                {
                    id = 0,
                    type = responseJson["code"].ToString() + " : " + responseJson["message"].ToString()
                };
            }
            return humandTime;
        }

        throw new Exception($"Failed to add clockOut EventLog: {content}");
    }

}