using Infrastructure.HttpClients.AccessControlClient;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Infrastructure.Repositories {
    public class AccessControlRepository {
        private readonly IAccessControlHttpClient _httpClient;
        public AccessControlRepository(IAccessControlHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool changeBaseUrl(string baseUrl)
        {
            return _httpClient.changeBaseUrl(baseUrl);
        }

        public async Task<EmployeeResponseSerialize> addEmployee(EmployeeSerialize employee)
        {
            var url = $"employee/add-employee";

            var response = await _httpClient.PostAsync(url, JsonSerializer.Serialize(employee));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var employeeRes = Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeResponseSerialize>(content);

                return employeeRes!;
            }

            throw new Exception($"Failed to Add employee: {content}");
        }
        public async Task<EmployeeResponseSerialize> updateEmployee(EmployeeSerialize employee)
        {
            var url = $"employee/update-employee";

            var response = await _httpClient.PostAsync(url, JsonSerializer.Serialize(employee));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var employeeRes = Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeResponseSerialize>(content);

                return employeeRes!;
            }

            throw new Exception($"Failed to Update employee: {content}");
        }

        public async Task<CaptureFingerResponse> addFingerPrintedEmployee(string employeeId)
        {
            var url = $"employee/add-finger-print";
            var fingerPrintData = new { IdentificationNumber = employeeId, FingerIndex = 1 };
            var response = await _httpClient.PostAsync(url, JsonSerializer.Serialize(fingerPrintData));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject resJson = JObject.Parse(content);
                var employeeRes = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptureFingerResponse>(resJson["data"].ToString());
                employeeRes.Message = resJson["message"].ToString();
                return employeeRes!;
            }
            throw new Exception($"Failed to Add fingerPrinted to empployee: {content}");

        }

        public async Task<EmployeeResponseSerialize> deleteFingerPrintedEmployee(string employeeId)
        {
            var url = $"employee/delete-finger-print";
            var fingerPrintData = new { IdentificationNumber = employeeId, FingerIndex = 1 };
            var response = await _httpClient.PostAsync(url, JsonSerializer.Serialize(fingerPrintData));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var employeeRes = new EmployeeResponseSerialize() { Message= content, StatusCode = (int) response.StatusCode };
                
                //Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeResponseSerialize>(content);

                return employeeRes!;
            }
            throw new Exception($"Failed to Delete fingerPrinted to empployee: {content}");

        }
    }
}