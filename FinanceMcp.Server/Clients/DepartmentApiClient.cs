using FinanceMcp.Server.Models;

namespace FinanceMcp.Server.Clients
{
    public class DepartmentApiClient
    {
        private readonly HttpClient _httpClient;

        public DepartmentApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Department?> GetDepartmentAsync(
            string departmentName,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"api/departments/{departmentName}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<Department>(
                    cancellationToken);
        }
    }
}
