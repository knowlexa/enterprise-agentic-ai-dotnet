using FinanceAgent.Api.Agents;
using FinanceAgent.Api;
using FinanceAgent.Api.Services;

public class EmployeeAgent : IAgent
{
    private readonly EmployeeAgentService _employeeAgentService;

    public string Name => "EmployeeAgent";

    public string Description =>
        "Handles employee information, employee lookup, department and manager information.";

    public EmployeeAgent(
        EmployeeAgentService employeeAgentService)
    {
        _employeeAgentService = employeeAgentService;
    }

    public async Task<string> ExecuteAsync(
        string input,
        CancellationToken cancellationToken = default)
    {
        return await _employeeAgentService.AskAsync(
            input,
            cancellationToken);
    }
}