using Azure.AI.Projects;
using Azure.Identity;
using FinanceAgent.Api.Configuration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;

namespace FinanceAgent.Api.Services;

public class FoundryLlmService : ILlmService
{
    private readonly FoundryOptions _options;

    public FoundryLlmService(
        IOptions<FoundryOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> GetResponseAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            throw new ArgumentException(
                "User prompt cannot be empty.",
                nameof(userPrompt));
        }

        var credential = new DefaultAzureCredential();

        var projectClient =
            new AIProjectClient(
                new Uri(_options.ProjectEndpoint),
                credential);

        var agent =
            projectClient.AsAIAgent(
                model: "gpt-4.1-mini",
                name: "RoutingAgent",
                instructions: systemPrompt);

        var response =
            await agent.RunAsync(
                userPrompt,
                cancellationToken: cancellationToken);

        return response.Text;
    }
}