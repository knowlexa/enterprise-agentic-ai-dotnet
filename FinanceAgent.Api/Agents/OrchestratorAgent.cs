
using System;
using System.Text.Json;
using FinanceAgent.Api;
using FinanceAgent.Api.Models;
using FinanceAgent.Api.Services;

namespace FinanceAgent.Api.Agents;

public class OrchestratorAgent
{
    private readonly IEnumerable<IAgent> _agents;
    private readonly ILlmService _llmService;

    public OrchestratorAgent(
        IEnumerable<IAgent> agents,
        ILlmService llmService)
    {
        _agents = agents;
        _llmService = llmService;
    }

    public async Task<string> ExecuteAsync(
        string userInput,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            throw new ArgumentException(
                "User input cannot be empty.",
                nameof(userInput));
        }

        // ---------------------------------------------------------
        // 1. Build specialist agent descriptions
        // ---------------------------------------------------------

        var agentDescriptions = string.Join(
            "\n",
            _agents.Select(a =>
                $"- {a.Name}: {a.Description}"));

        // ---------------------------------------------------------
        // 2. Build orchestration prompt
        // ---------------------------------------------------------

        var systemPrompt = $$"""
            You are an orchestration agent.

            Your responsibility is to analyze the user's request
            and determine which specialist agents are required.

            Available specialist agents:

            {{agentDescriptions}}

            Rules:

            1. For a simple request, select exactly one agent.

            2. For a request requiring information from multiple
               domains, select multiple agents.

            3. Each selected agent must receive a specific task.

            4. Do not answer the user's question yourself.

            5. Return only valid JSON.

            6. AgentName must exactly match one of the available
               agent names.

            7. Do not create agents that are not listed above.

            8. If only one agent is required,
               requiresCollaboration must be false.

            9. If multiple agents are required,
               requiresCollaboration must be true.

            Response format:

            {
                "requiresCollaboration": true,
                "tasks": [
                    {
                        "agentName": "EmployeeAgent",
                        "task": "Get employee information."
                    },
                    {
                        "agentName": "FinanceAgent",
                        "task": "Get financial information."
                    }
                ],
                "reason": "The request requires information from multiple domains."
            }
            """;

        // ---------------------------------------------------------
        // 3. Ask LLM to create execution plan
        // ---------------------------------------------------------

        var routingResult =
            await _llmService.GetResponseAsync(
                systemPrompt,
                userInput,
                cancellationToken);

        // ---------------------------------------------------------
        // 4. Deserialize routing decision
        // ---------------------------------------------------------

        AgentRoutingDecision? decision;

        try
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            decision =
                JsonSerializer.Deserialize<AgentRoutingDecision>(
                    routingResult,
                    jsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "The orchestration LLM returned invalid JSON.",
                ex);
        }

        // ---------------------------------------------------------
        // 5. Validate routing decision
        // ---------------------------------------------------------

        if (decision == null)
        {
            throw new InvalidOperationException(
                "The orchestration LLM returned an empty decision.");
        }

        if (decision.Tasks == null ||
            decision.Tasks.Count == 0)
        {
            throw new InvalidOperationException(
                "The orchestration LLM did not select any agents.");
        }

        // ---------------------------------------------------------
        // 6. Validate selected agents
        // ---------------------------------------------------------

        foreach (var task in decision.Tasks)
        {
            if (string.IsNullOrWhiteSpace(task.AgentName))
            {
                throw new InvalidOperationException(
                    "The orchestration decision contains an empty agent name.");
            }

            if (string.IsNullOrWhiteSpace(task.Task))
            {
                throw new InvalidOperationException(
                    $"No task was provided for agent '{task.AgentName}'.");
            }

            var agentExists =
                _agents.Any(a =>
                    a.Name.Equals(
                        task.AgentName,
                        StringComparison.OrdinalIgnoreCase));

            if (!agentExists)
            {
                throw new InvalidOperationException(
                    $"Agent '{task.AgentName}' was not found.");
            }
        }

        // ---------------------------------------------------------
        // 7. Execute specialist agents
        // ---------------------------------------------------------

        var executionTasks =
            decision.Tasks.Select(async task =>
            {
                var selectedAgent =
                    _agents.First(a =>
                        a.Name.Equals(
                            task.AgentName,
                            StringComparison.OrdinalIgnoreCase));

                var response =
                    await selectedAgent.ExecuteAsync(
                        task.Task,
                        cancellationToken);

                return new AgentTaskResult
                {
                    AgentName = selectedAgent.Name,
                    Task = task.Task,
                    Response = response
                };
            });

        // ---------------------------------------------------------
        // 8. Execute agents in parallel
        // ---------------------------------------------------------

        var results =
            await Task.WhenAll(executionTasks);

        // ---------------------------------------------------------
        // 9. Single-agent request
        // ---------------------------------------------------------

        if (!decision.RequiresCollaboration &&
            results.Length == 1)
        {
            return results[0].Response;
        }

        // ---------------------------------------------------------
        // 10. Multiple-agent request
        // ---------------------------------------------------------

        return await SynthesizeAsync(
            userInput,
            results,
            cancellationToken);
    }

    // =============================================================
    // SYNTHESIS
    // =============================================================

    private async Task<string> SynthesizeAsync(
        string originalQuestion,
        IEnumerable<AgentTaskResult> results,
        CancellationToken cancellationToken)
    {
        var agentResults = string.Join(
            "\n\n",
            results.Select(r =>
                $"""
                Agent: {r.AgentName}

                Task:
                {r.Task}

                Result:
                {r.Response}
                """));

        var systemPrompt = """
            You are a response synthesis agent.

            Your responsibility is to combine the results
            returned by multiple specialist agents into one
            accurate response for the user.

            Rules:

            1. Use only information provided by the specialist agents.

            2. Do not invent information.

            3. Do not make assumptions when information is missing.

            4. Preserve important details from each specialist.

            5. Remove unnecessary duplication.

            6. If specialist results conflict, clearly mention
               the conflict instead of inventing an answer.

            7. Answer the original user question directly.

            8. Return a clear natural-language answer.
            """;

        var userPrompt = $"""
            Original user question:

            {originalQuestion}

            Specialist agent results:

            {agentResults}
            """;

        return await _llmService.GetResponseAsync(
            systemPrompt,
            userPrompt,
            cancellationToken);
    }
}
