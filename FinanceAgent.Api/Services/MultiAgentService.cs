using FinanceAgent.Api.Agents;

namespace FinanceAgent.Api.Services;

public class MultiAgentService
{
    private readonly IEnumerable<IAgent> _agents;

    public MultiAgentService(
        IEnumerable<IAgent> agents)
    {
        _agents = agents;
    }

    public async Task<string> ExecuteAsync(
        string agentName,
        string input,
        CancellationToken cancellationToken = default)
    {
        var agent = _agents.FirstOrDefault(
            x => x.Name.Equals(
                agentName,
                StringComparison.OrdinalIgnoreCase));

        if (agent == null)
        {
            throw new InvalidOperationException(
                $"Agent '{agentName}' not found.");
        }

        return await agent.ExecuteAsync(
            input,
            cancellationToken);
    }
}