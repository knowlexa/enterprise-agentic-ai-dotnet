using Microsoft.AspNetCore.Mvc;

namespace FinanceAgent.Api
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        [HttpGet("{id:int}")]
        public IActionResult GetEmployee(int id)
        {
            var employees = new Dictionary<int, Employee>
            {
                [101] = new Employee(
                    101,
                    "Vinay",
                    "Finance",
                    "Solution Architect"),

                [102] = new Employee(
                    102,
                    "Rahul",
                    "Technology",
                    "Senior Developer"),

                [103] = new Employee(
                    103,
                    "Priya",
                    "HR",
                    "HR Manager")
            };

            if (!employees.TryGetValue(id, out var employee))
            {
                return NotFound(new
                {
                    message = $"Employee {id} not found."
                });
            }

            return Ok(employee);
        }
    }

}
