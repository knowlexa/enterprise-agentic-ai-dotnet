// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using ModelContextProtocol.Client;

var transport =
    new HttpClientTransport(
        new HttpClientTransportOptions
        {
            Endpoint =
                new Uri("http://localhost:5215")
        });

await using var client =
    await McpClient.CreateAsync(transport);

Console.WriteLine(
    $"Connected to: {client.ServerInfo?.Name}");

Console.WriteLine();
Console.WriteLine("Available tools:");

var tools = await client.ListToolsAsync();


foreach (var tool in tools)
{
    Console.WriteLine(
        $"- {tool.Name}: {tool.Description}");
}

var result =
    await client.CallToolAsync(
        "get_employee",
        new Dictionary<string, object?>
        {
            ["employeeId"] = 101
        });

Console.WriteLine(result.Content.FirstOrDefault());

Console.ReadKey();