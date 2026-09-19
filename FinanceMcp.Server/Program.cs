using FinanceMcp.Server.Clients;
using FinanceMcp.Server.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<EmployeeApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri("http://localhost:5267/");
    });

builder.Services.AddHttpClient<DepartmentApiClient>(
    client =>
    {
        client.BaseAddress =
            new Uri("http://localhost:5267/");
    });


builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<EmployeeTools>();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapMcp();
app.Run();
