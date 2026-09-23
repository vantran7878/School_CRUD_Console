using System.Reflection.Metadata;
using School_CRUD_console.Interfaces;
using School_CRUD_console.Repository;
using School_CRUD_console.Subjects;

namespace School_CRUD_console.Models;

public class School
{
    private TeacherRepository _teacherRepo = new();
    private StudentRepository _studentRepo = new();

    private ILogger _logger;
    private IGPACalculator _GPACalc;

    public School(ILogger logger, IGPACalculator GPACalc)
    {
        _logger = logger;
        _GPACalc = GPACalc;
    }

    public async Task InitializeDataAsync(int studentCount = 10)
    {
        await _logger.LogWarning("=== KHỞI TẠO DỮ LIỆU TRƯỜNG HỌC ===");

        Subject[] subjects = (Subject[])Enum.GetValues(typeof(Subject));

        foreach (var sub in subjects)
        {
            Teacher t = TeacherFactory.CreateRandomTeacher(_logger, sub);
            _teacherRepo.Add(t);
        }
        await _logger.LogSuccess($"Đã tạo {_teacherRepo.GetAll().Count} giáo viên cho các môn học.");

        for (int i = 0; i < studentCount; ++i)
        {
            Student s = StudentFactory.CreateRandomStudent(_logger);
            _studentRepo.Add(s);
        }
        await _logger.LogSuccess($"Đã tuyển {studentCount} học sinh vào trường.");
    }
    // Chạy mô phỏng N học kỳ
    public async Task StartSimulationAsync(int totalSemesters)
    {
        for (int i = 1; i <= totalSemesters; i++)
        {
            Semester semester = new Semester(i, _teacherRepo, _studentRepo, _logger, _GPACalc);
            await semester.RunAsync();
        }
    }
}