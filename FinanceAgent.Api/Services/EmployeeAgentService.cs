using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using FinanceAgent.Api.Configuration;

namespace FinanceAgent.Api;

public class EmployeeAgentService
{
    private readonly FoundryOptions _options;
    private readonly McpClient _mcpClient;

    public EmployeeAgentService(
        IOptions<FoundryOptions> options,
        McpClient mcpClient)
    {
        _options = options.Value;
        _mcpClient = mcpClient;
    }

    public async Task<string> AskAsync(
        string question,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException(
                "Question cannot be empty.",
                nameof(question));
        }

        // 1. Discover MCP tools
        var mcpTools =
            await _mcpClient.ListToolsAsync();

        // 2. Create Azure credential
        var credential =
            new DefaultAzureCredential();

        // 3. Create Foundry project client
        var projectClient =
            new AIProjectClient(
                new Uri(_options.ProjectEndpoint),
                credential);

        // 4. Create Employee AI Agent
        var agent =
            projectClient.AsAIAgent(
                model: "gpt-4.1-mini",

                name: "EmployeeAgent",

                instructions: """
                    You are an Employee Information Agent.

                    Your job is to answer questions about employees
                    and departments.

                    You have access to tools provided through MCP.

                    Use the appropriate MCP tool whenever employee
                    or department information is required.

                    Never invent employee information.

                    If information cannot be found,
                    clearly tell the user.
                    """,

                tools: [.. mcpTools]
            );

        // 5. Let the LLM decide whether to call a tool
        var response =
            await agent.RunAsync(
                question,
                cancellationToken: cancellationToken);

        return response.Text;
    }
}