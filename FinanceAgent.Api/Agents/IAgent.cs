namespace FinanceAgent.Api.Agents;

public interface IAgent
{
    string Name { get; }

    string Description { get; }

    Task<string> ExecuteAsync(
        string input,
        CancellationToken cancellationToken = default);
}