using System.Net.Http.Json;
namespace FinanceAgent.Api
{
    public class DepartmentApiClient
    {
        private readonly HttpClient _httpClient;

        public DepartmentApiClient(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Department?> GetDepartmentAsync(
            string departmentName)
        {
            Console.WriteLine(
                $"[HTTP] GET api/departments/{departmentName}");

            var response =
                await _httpClient.GetAsync(
                    $"api/departments/{departmentName}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<Department>();
        }
    }
}
