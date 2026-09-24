namespace School_CRUD_console.Logger;

using School_CRUD_console.Interfaces;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

public class JSONItem
{
    public string Message { get; set; }
    public string Level { get; set; }
    public string Timestamp { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    public JSONItem(string msg, string level)
    {
        Message = msg;
        Level = level;
    }
}

public class JSON_Logger : ILogger
{

    private readonly string _humanLogFilepath = "./JSON Log/human_log.jsonl";
    private readonly string _codeLogFilepath = "./JSON Log/code_log.jsonl";

    public JSON_Logger()
    {
        Directory.CreateDirectory("./JSON Log");
    }
    private async Task WriteLog(string msg, string level)
    {
        var options_human = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        };

        JSONItem item = new(msg, level);
        string jsonString = JsonSerializer.Serialize(item, options_human);

        await File.AppendAllTextAsync(_humanLogFilepath, jsonString + Environment.NewLine);

        var options_code = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        };
        jsonString = JsonSerializer.Serialize(item, options_code);

        await File.AppendAllTextAsync(_codeLogFilepath, jsonString + Environment.NewLine);

    }
    public async Task LogInfo(string msg) => await WriteLog(msg, "INFO");
    public async Task LogWarning(string msg) => await WriteLog(msg, "WARNING");
    public async Task LogError(string msg) => await WriteLog(msg, "ERROR");
    public async Task LogSuccess(string msg) => await WriteLog(msg, "SUCCESS");
}