namespace FinanceAgent.Api.Services;

public interface ILlmService
{
    Task<string> GetResponseAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default);
}