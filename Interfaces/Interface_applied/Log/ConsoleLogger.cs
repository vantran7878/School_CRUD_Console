namespace School_CRUD_console.Logger;

using School_CRUD_console.Interfaces;

public class ConsoleLogger : ILogger
{
    string date => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    string time => DateTime.Now.ToString("HH:mm:ss");

    public Task LogInfo(string msg)
    {
        Console.WriteLine($"INFO: [{date}], {msg}");
        return Task.CompletedTask;
    }
    public Task LogWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARN: [{date}], {msg}");
        Console.ResetColor();
        return Task.CompletedTask;
    }
    public Task LogError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERR: [{date}], {msg}");
        Console.ResetColor();
        return Task.CompletedTask;
    }
    public Task LogSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"SUCCESS: [{date}], {msg}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

}