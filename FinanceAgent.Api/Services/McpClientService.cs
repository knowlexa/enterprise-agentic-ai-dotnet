using ModelContextProtocol.Client;

namespace FinanceAgent.Api.Services;

public class McpClientService : IAsyncDisposable
{
    private readonly McpClient _client;

    public McpClientService(McpClient client)
    {
        _client = client;
    }

    public async Task<IList<McpClientTool>> GetToolsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _client.ListToolsAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
    }
}