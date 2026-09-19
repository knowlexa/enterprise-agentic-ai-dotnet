using Microsoft.AspNetCore.Mvc;

namespace FinanceAgent.Api
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentsController : ControllerBase
    {
        [HttpGet("{name}")]
        public IActionResult GetDepartment(string name)
        {
            var departments =
                new Dictionary<string, Department>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    ["Finance"] = new(
                        "Finance",
                        "John Smith",
                        "London"),

                    ["Technology"] = new(
                        "Technology",
                        "Sarah Wilson",
                        "Bangalore"),

                    ["HR"] = new(
                        "HR",
                        "Michael Brown",
                        "New York")
                };

            if (!departments.TryGetValue(
                    name,
                    out var department))
            {
                return NotFound(new
                {
                    message =
                        $"Department '{name}' not found."
                });
            }

            return Ok(department);
        }
    }
}
