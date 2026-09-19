using System.Net.Http.Json;

namespace FinanceAgent.Api
{
    public class EmployeeApiClient
    {
        private readonly HttpClient _httpClient;

        public EmployeeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Employee?> GetEmployeeAsync(
            int employeeId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/employees/{employeeId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<Employee>();
        }
    }

    //public record Employee(
    //    int Id,
    //    string Name,
    //    string Department,
    //    string Role);
}

