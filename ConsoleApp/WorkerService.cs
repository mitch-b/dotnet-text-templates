using System.Text.Json;
using ConsoleApp.Models;
using ConsoleApp.Models.Configuration;
using ConsoleApp.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp;

internal class WorkerService(
    ILogger<WorkerService> logger,
    IDemoService demoService, 
    IOptions<ConsoleAppSettings> options) : IHostedService
{
    private readonly ILogger<WorkerService> _logger = logger;
    private readonly IDemoService _demoService = demoService;
    private readonly IOptions<ConsoleAppSettings> _options = options;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker started");
        var oldObj = new SampleModel { Prop1 = "hi2", Prop2 = 3 };
        var newObj = new SampleModel { Prop1 = "hi2", Prop2 = 4 };
        
        Console.WriteLine("object 'a': {0}", JsonSerializer.Serialize(oldObj));
        Console.WriteLine("object 'b': {0}", JsonSerializer.Serialize(newObj));

        do
        {

            Console.WriteLine("Enter expression to evaluate :");
            var expression = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(expression))
            {
                _logger.LogInformation("Exiting worker");
                break;
            }
            
            var expressionResult = await _demoService.EvaluateExpression<bool>(
                expression,
                (oldObj, "a"),
                (newObj, "b"));
            Console.WriteLine("Result: {0}", expressionResult);
            // await Task.Delay(3000, cancellationToken);
        } while (!cancellationToken.IsCancellationRequested);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
