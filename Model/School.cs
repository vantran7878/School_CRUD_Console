using System.Reflection.Metadata;
using School_CRUD_console.Interfaces;
using School_CRUD_console.Repository;
using School_CRUD_console.Services;
using School_CRUD_console.Subjects;
using School_CRUD_console.Queue;

namespace School_CRUD_console.Models;

public class School
{
    private TeacherRepository _teacherRepo = new();
    private StudentRepository _studentRepo = new();

    private ILogger _logger;
    private IGPACalculator _GPACalc;

    private readonly PersistenceService _persistence;

    private readonly SchoolEventQueue _queue;

    public School(ILogger logger, IGPACalculator GPACalc, PersistenceService persistence, SchoolEventQueue queue)
    {
        _logger = logger;
        _GPACalc = GPACalc;
        _persistence = persistence;
        _queue = queue;
    }

    public async Task InitializeDataAsync(int studentCount = 10)
    {
        await _logger.LogWarning("=== KHỞI TẠO DỮ LIỆU TRƯỜNG HỌC ===");

        Subject[] subjects = (Subject[])Enum.GetValues(typeof(Subject));

        foreach (var sub in subjects)
        {
            Teacher t = TeacherFactory.CreateRandomTeacher(_logger, sub);
            await _teacherRepo.AddAsync(t);
        }
        await _logger.LogSuccess($"Đã tạo {(await _teacherRepo.GetAllAsync()).Count} giáo viên cho các môn học.");

        for (int i = 0; i < studentCount; ++i)
        {
            Student s = StudentFactory.CreateRandomStudent(_logger);
            await _studentRepo.AddAsync(s);
        }
        await _logger.LogSuccess($"Đã tuyển {studentCount} học sinh vào trường.");
    }
    // Chạy mô phỏng N học kỳ
    public async Task StartSimulationAsync(int totalSemesters)
    {
        for (int i = 1; i <= totalSemesters; i++)
        {
            Semester semester = new Semester(i, _teacherRepo, _studentRepo, _logger, _GPACalc, _queue);
            await semester.RunAsync();
        }
    }

    public async Task<bool> LoadSavedDataAsync()
    {
        await _persistence.LoadAllAsync(_studentRepo, _teacherRepo);

        var students = await _studentRepo.GetAllAsync();
        var teachers = await _teacherRepo.GetAllAsync();

        return students.Any() && teachers.Any();

    }

    public async Task SaveAllDataAsync()
    {
        await _persistence.SaveAllAsync(_studentRepo, _teacherRepo);
    }
}