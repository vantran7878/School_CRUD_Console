using System.Threading.Tasks;
using School_CRUD_console.Interfaces;


public class CompositeLogger : ILogger
{

    private readonly List<ILogger> _listLogger = new List<ILogger>();

    public CompositeLogger(params ILogger[] loggers)
    {
        _listLogger.AddRange(loggers);
    }

    public void AddLogger(ILogger logger)
    {
        _listLogger.Add(logger);
    }
    public async Task LogInfo(string msg)
    {
        foreach (var logger in _listLogger)
        {
            await logger.LogInfo(msg);
        }
    }
    public async Task LogWarning(string msg)
    {
        foreach (var logger in _listLogger)
        {
            await logger.LogWarning(msg);
        }

    }
    public async Task LogError(string msg)
    {
        foreach (var logger in _listLogger)
        {
            await logger.LogError(msg);
        }
    }
    public async Task LogSuccess(string msg)
    {
        foreach (var logger in _listLogger)
        {
            await logger.LogSuccess(msg);
        }
    }
}