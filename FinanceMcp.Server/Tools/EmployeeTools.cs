using ModelContextProtocol.Server;
using System.ComponentModel;
using FinanceMcp.Server.Clients;

namespace FinanceMcp.Server.Tools
{
    [McpServerToolType]
    public class EmployeeTools
    {
        private readonly EmployeeApiClient _employeeApiClient;
        private readonly DepartmentApiClient _departmentApiClient;

        public EmployeeTools(
            EmployeeApiClient employeeApiClient,
            DepartmentApiClient departmentApiClient)
        {
            _employeeApiClient = employeeApiClient;
            _departmentApiClient = departmentApiClient;
        }

        [McpServerTool]
        [Description(
            "Gets employee information using the employee ID.")]
        public async Task<object?> GetEmployee(
            int employeeId,
            CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(
                $"[MCP TOOL] get_employee({employeeId})");

            var employee =
                await _employeeApiClient.GetEmployeeAsync(
                    employeeId,
                    cancellationToken);

            if (employee == null)
            {
                return new
                {
                    success = false,
                    errorCode = "EMPLOYEE_NOT_FOUND",
                    message =
                        $"Employee {employeeId} was not found."
                };
            }

            return new
            {
                success = true,
                employee
            };
        }

        [McpServerTool]
        [Description(
            "Gets department information including manager and location.")]
        public async Task<object?> GetDepartment(
            string departmentName,
            CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(
                $"[MCP TOOL] get_department({departmentName})");

            var department =
                await _departmentApiClient.GetDepartmentAsync(
                    departmentName,
                    cancellationToken);

            if (department == null)
            {
                return new
                {
                    success = false,
                    errorCode = "DEPARTMENT_NOT_FOUND",
                    message =
                        $"Department '{departmentName}' was not found."
                };
            }

            return new
            {
                success = true,
                department
            };
        }
    }
}
