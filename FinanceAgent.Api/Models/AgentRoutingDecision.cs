namespace FinanceAgent.Api.Models;

public class AgentRoutingDecision
{
    public bool RequiresCollaboration { get; set; }

    public List<AgentTask> Tasks { get; set; } = [];

    public string Reason { get; set; } = string.Empty;
}