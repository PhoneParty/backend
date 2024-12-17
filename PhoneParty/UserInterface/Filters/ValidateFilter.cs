using System.Drawing.Printing;
using Microsoft.AspNetCore.SignalR;

namespace UserInterface.Filters;

public class ValidateFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        foreach (var argument in invocationContext.HubMethodArguments.Select((arg, index) => new { arg, index }))
        {
            if (argument.arg == null)
            {
                Console.Error.WriteLine(
                    $"[ERROR] Argument {argument.index} in method '{invocationContext.HubMethodName}' is null.");
            }
        }

        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(
                $"[EXCEPTION] An error occurred in method '{invocationContext.HubMethodName}': {ex.Message}");
            return null;
        }
    }
}