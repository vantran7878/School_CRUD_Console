using System.Dynamic;
using School_CRUD_console.Models;

namespace School_CRUD_console.Events;

public abstract class SchoolEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.Now;
    public abstract string Message { get; }
}

// Student Finish Exam

