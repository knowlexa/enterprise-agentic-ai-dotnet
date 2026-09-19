using FinanceAgent.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAgent.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly EmployeeAgentService _agent;

        public FinanceController(
            EmployeeAgentService agent)
        {
            _agent = agent;
        }
        [HttpPost("ask")]
        public async Task<IActionResult> Ask(
        [FromBody] FinanceQuestion request)
        {
            var result = await _agent.AskAsync(
                request.Question);

            return Ok(new
            {
                answer = result
            });
        }
    }
    public record FinanceQuestion(string Question);
}
