using System.Dynamic;
using School_CRUD_console.Enums;
using School_CRUD_console.Models;
using School_CRUD_console.Subjects;

namespace School_CRUD_console.Events;

public abstract class SchoolEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.Now;
    public abstract string Message { get; }
}

// Student Finish Exam
public class ExamCompleteEvent : SchoolEvent
{
    public string StudentName { get; }
    public Subject Subject { get; }
    public int Grade { get; }

    public ExamDifficult Difficulty { get; }

    public ExamCompleteEvent(string studentName, Subject subject, int grade, ExamDifficult difficulty)
    {
        (StudentName, Subject, Grade, Difficulty) = (studentName, subject, grade, difficulty);
    }

    public override string Message => $"[EXAM COMPLETE] Sinh viên {StudentName} đã thi xong môn {Subject} (Độ khó: {Difficulty}) - Điểm: {Grade}";
}

public class TeacherResignedEvent : SchoolEvent
{
    public string TeacherName { get; }
    public Subject Subject { get; }
    public int FinalSalary { get; }

    public TeacherResignedEvent(string teacherName, Subject subject, int finalSalary) => (TeacherName, Subject, FinalSalary) = (teacherName, subject, finalSalary);

    public override string Message => $"[TEACHER RESIGNED] Giáo viên {TeacherName} môn {Subject} nghỉ việc do lương ({FinalSalary}) < ngưỡng!";
}

public class TeacherHiredEvent : SchoolEvent
{
    public string TeacherName { get; }
    public Subject Subject { get; }

    public TeacherHiredEvent(string teacherName, Subject subject) => (TeacherName, Subject) = (teacherName, subject);

    public override string Message => $"[TEACHER HIRED] Đã tuyển giáo viên mới {TeacherName} dạy môn {Subject}.";

}
