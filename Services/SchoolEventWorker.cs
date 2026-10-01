namespace School_CRUD_console.Services;

using School_CRUD_console.Queue;
using School_CRUD_console.Events;
using School_CRUD_console.Interfaces;


public class SchoolEventWorker
{
    private readonly SchoolEventQueue _queue;
    private readonly ILogger _logger;

    public SchoolEventWorker(SchoolEventQueue queue, ILogger logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        await _logger.LogInfo("[WORKER STARTED] Background Event Worker đã bắt đầu lắng nghe Queue...");

        try
        {

            await foreach (var schoolEvent in _queue.ReadAllAsync(cancellationToken))
            {
                await ProcessEventAsync(schoolEvent);
            }
        }
        catch (OperationCanceledException)
        {
            await _logger.LogWarning("[WORKER STOPPING] Nhận tín hiệu CancellationToken dừng Worker...");
        }

        await _logger.LogInfo("[WORKER STOPPED] Background Event Worker đã kết thúc an toàn (Graceful Shutdown).");


    }

    public async Task ProcessEventAsync(SchoolEvent evt)
    {
        switch (evt)
        {
            case ExamCompleteEvent examEvt:
                await _logger.LogInfo(examEvt.Message);
                break;

            case TeacherResignedEvent resignedEvt:
                await _logger.LogInfo(resignedEvt.Message);
                break;

            case TeacherHiredEvent hireEvt:
                await _logger.LogInfo(hireEvt.Message);
                break;

            default:
                await _logger.LogInfo(evt.Message);
                break;

        }
    }
}