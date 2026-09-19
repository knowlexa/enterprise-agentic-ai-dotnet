using System.ComponentModel;
using Microsoft.Agents.AI;

namespace FinanceAgent.Api
{
    public class EmployeeTools
    {
        private readonly EmployeeApiClient _employeeApi;
        private readonly DepartmentApiClient _departmentApiClient;

        public EmployeeTools(EmployeeApiClient employeeApi, DepartmentApiClient departmentApiClient)
        {
            _employeeApi = employeeApi;
            _departmentApiClient = departmentApiClient;
        }

        [Description("Gets employee information using the employee ID.")]
        public async Task<Employee?> GetEmployee(
        int employeeId)
        {
            Console.WriteLine(
                $"[TOOL CALL] get_employee({employeeId})");

            var employee =
                await _employeeApi
                    .GetEmployeeAsync(employeeId);

            Console.WriteLine(
                employee == null
                    ? $"[TOOL RESULT] Employee {employeeId} NOT FOUND"
                    : $"[TOOL RESULT] {employee.Name}");

            return employee;
        }
        [Description(
       "Gets department information including department manager and location.")]
        public async Task<Department?> GetDepartment(
       string departmentName)
        {
            Console.WriteLine(
                $"[TOOL CALL] get_department({departmentName})");

            var department =
                await _departmentApiClient
                    .GetDepartmentAsync(departmentName);

            Console.WriteLine(
                department == null
                    ? $"[TOOL RESULT] Department {departmentName} NOT FOUND"
                    : $"[TOOL RESULT] {department.Name}");

            return department;
        }
    }
}
