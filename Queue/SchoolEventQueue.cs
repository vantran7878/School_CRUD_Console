namespace School_CRUD_console.Queue;

using System.Threading.Channels;
using School_CRUD_console.Events;
using School_CRUD_console.Models;

public class SchoolEventQueue
{
    private readonly Channel<SchoolEvent> _channel = Channel.CreateUnbounded<SchoolEvent>(new UnboundedChannelOptions
    {
        SingleReader = true,
    });

    public async ValueTask PublishAsync(SchoolEvent schoolEvent)
    {
        await _channel.Writer.WriteAsync(schoolEvent);
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }

    public IAsyncEnumerable<SchoolEvent> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}