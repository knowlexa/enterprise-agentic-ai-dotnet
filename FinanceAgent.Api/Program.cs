using FinanceAgent.Api;
using FinanceAgent.Api.Agents;
using FinanceAgent.Api.Configuration;
using FinanceAgent.Api.Services;
using ModelContextProtocol.Client;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Configuration
// ----------------------------------------------------

builder.Services.Configure<FoundryOptions>(
    builder.Configuration.GetSection("Foundry"));


// ----------------------------------------------------
// Controllers + Swagger
// ----------------------------------------------------

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// ----------------------------------------------------
// MCP Client
// ----------------------------------------------------

builder.Services.AddSingleton<McpClient>(sp =>
{
    var transport =
        new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint =
                    new Uri("http://localhost:5215/")
            });

    return McpClient.CreateAsync(transport)
        .GetAwaiter()
        .GetResult();
});

builder.Services.AddSingleton<McpClientService>();


// ----------------------------------------------------
// Employee / Department APIs
// ----------------------------------------------------

builder.Services.AddHttpClient<EmployeeApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri("https://localhost:7243/");
    });

builder.Services.AddHttpClient<DepartmentApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri("https://localhost:7243/");
    });


// ----------------------------------------------------
// Multi-Agent
// ----------------------------------------------------

builder.Services.AddScoped<EmployeeAgentService>();

// Specialist Agents
builder.Services.AddScoped<IAgent, EmployeeAgent>();
builder.Services.AddScoped<IAgent, FinancesAgent>();
builder.Services.AddScoped<IAgent, RagAgent>();

// LLM
builder.Services.AddScoped<ILlmService, FoundryLlmService>();

// Orchestrator
builder.Services.AddScoped<OrchestratorAgent>();


// ----------------------------------------------------
// Build
// ----------------------------------------------------

var app = builder.Build();


// ----------------------------------------------------
// Swagger
// ----------------------------------------------------

app.UseSwagger();

app.UseSwaggerUI();


// ----------------------------------------------------
// HTTPS
// ----------------------------------------------------

app.UseHttpsRedirection();


// ----------------------------------------------------
// Controllers
// ----------------------------------------------------

app.MapControllers();


// ----------------------------------------------------
// MCP Test Endpoint
// ----------------------------------------------------

app.MapGet("/mcp-tools", async (
    McpClientService mcpClientService) =>
{
    var tools =
        await mcpClientService.GetToolsAsync();

    return tools.Select(x => new
    {
        x.Name,
        x.Description
    });
});


app.Run();