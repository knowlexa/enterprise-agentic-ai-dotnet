using FinanceMcp.Server.Models;

namespace FinanceMcp.Server.Clients
{
    public class EmployeeApiClient
    {
        private  HttpClient _httpClient;

        public EmployeeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Employee?> GetEmployeeAsync(
            int employeeId,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"api/employees/{employeeId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<Employee>(
                    cancellationToken);
        }
    }
}
