using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using ConsoleApp.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace ConsoleApp.Services;

internal interface IDemoService
{
    Task<T> EvaluateExpression<T>(string expression, params (object obj, string objName)[] objects);
}

internal class DemoService(ILogger<DemoService> logger,
        IOptions<ConsoleAppSettings> consoleAppSettings) : IDemoService
{
    private readonly ILogger<DemoService> _logger = logger;
    private readonly IOptions<ConsoleAppSettings> _consoleAppSettings = consoleAppSettings;
    private static readonly ScriptOptions _scriptOptions = ScriptOptions.Default;
    private static readonly ConcurrentDictionary<string, Script<object>> _scriptCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

    public async Task<T> EvaluateExpression<T>(string expression, params (object obj, string objName)[] objects)
    {
        var replacedExpression = expression;
        try
        {
            foreach (var (obj, objName) in objects)
            {
                replacedExpression = ReplaceNameofReferences(replacedExpression, obj, objName);
            }

            _logger.LogDebug("Evaluating expression: '{replacedExpression}' from '{expression}'", replacedExpression, expression);
            var script = _scriptCache.GetOrAdd(replacedExpression, expr => CSharpScript.Create<object>(expr, _scriptOptions));
            var result = await script.RunAsync();
            return (T)result.ReturnValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating expression: {replacedExpression}", replacedExpression);
            throw;
        }
    }

    private string ReplaceNameofReferences(string expression, object obj, string objName)
    {
        if (obj == null) return expression;

        var type = obj.GetType();
        var properties = _propertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(obj);

            if (propertyValue != null)
            {
                string replacementValue;

                if (propertyValue is string)
                {
                    replacementValue = $"\"{propertyValue}\"";
                }
                else if (propertyValue is DateTime dateTimeValue)
                {
                    replacementValue = $"DateTime.Parse(\"{dateTimeValue:O}\")"; // ISO 8601 format
                }
                else if (propertyValue is bool boolValue)
                {
                    replacementValue = boolValue.ToString().ToLower();
                }
                else
                {
                    replacementValue = propertyValue.ToString();
                }

                expression = Regex.Replace(expression, $@"\b{objName}\.{propertyName}\b", replacementValue);
            }
        }

        return expression;
    }

    public void WriteWelcomeMessage()
    {
        _logger.LogDebug("Saying hello");
        _logger.LogInformation(_consoleAppSettings.Value?.WelcomeMessage);
    }
}
