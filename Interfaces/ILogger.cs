namespace School_CRUD_console.Interfaces;

using School_CRUD_console.Models;
using System.Threading.Tasks;

public interface ILogger
{
    Task LogInfo(string msg);
    Task LogWarning(string msg);
    Task LogError(string msg);
    Task LogSuccess(string msg);
}