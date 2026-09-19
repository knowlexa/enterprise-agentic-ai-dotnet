using FinanceAgent.Api.Agents;

public class RagAgent : IAgent
{
    public string Name => "RagAgent";

    public string Description =>
        "Answers questions using company documents, policies and knowledge base.";

    public async Task<string> ExecuteAsync(
        string input,
        CancellationToken cancellationToken = default)
    {
        return $"RagAgent received: {input}";
    }
}