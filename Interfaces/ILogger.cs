namespace School_CRUD_console.Interface;

using School_CRUD_console.Model;
using System.Threading.Tasks;

public interface ILogger
{
    Task LogInfo(string msg);
    Task LogWarning(string msg);
    Task LogError(string msg);
    Task LogSuccess(string msg);
}