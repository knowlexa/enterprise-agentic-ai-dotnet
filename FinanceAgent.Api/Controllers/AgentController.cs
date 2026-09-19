using FinanceAgent.Api.Agents;
using FinanceAgent.Api.Models;
using FinanceAgent.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAgent.Api.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController : ControllerBase
{
    //private readonly MultiAgentService _multiAgentService;
    private readonly OrchestratorAgent _orchestratorAgent;

    public AgentController(OrchestratorAgent orchestratorAgent)
    {
        //_multiAgentService = multiAgentService;
        _orchestratorAgent = orchestratorAgent;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask(
    [FromBody] AgentRequest request,
    CancellationToken cancellationToken)
    {
        var response =
            await _orchestratorAgent.ExecuteAsync(
                request.Input,
                cancellationToken);

        return Ok(new
        {
            response
        });
    }
}