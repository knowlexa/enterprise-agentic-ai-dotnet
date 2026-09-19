using FinanceAgent.Api.Agents;

public class FinancesAgent : IAgent
{
    public string Name => "FinanceAgent";

    public string Description =>
        "Handles finance, revenue, billing and financial business questions.";

    public async Task<string> ExecuteAsync(
        string input,
        CancellationToken cancellationToken = default)
    {
        return $"FinanceAgent received: {input}";
    }
}