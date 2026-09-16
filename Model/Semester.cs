namespace School_CRUD_console.Model;

using School_CRUD_console.Interface;
using School_CRUD_console.Factory;
using School_CRUD_console.Subjects;
using School_CRUD_console.Repository;

public class Semester
{
    private readonly int _semesterNumber;
    private readonly TeacherRepository _teacherRepo;
    private readonly StudentRepository _studentRepo;
    private readonly ILogger _logger;

    private readonly int _fireThreshold = 5500;

    private readonly IGPACalculator _GPACalc;

    public Semester(int semesterNumber, TeacherRepository teacherRepository, StudentRepository studentRepository, ILogger logger, IGPACalculator GPAcalc)
    {
        _semesterNumber = semesterNumber;
        _teacherRepo = teacherRepository;
        _studentRepo = studentRepository;
        _logger = logger;
        _GPACalc = GPAcalc;
    }

    public async Task RunAsync()
    {
        await _logger.LogInfo($"\n================ BẮT ĐẦU HỌC KỲ {_semesterNumber} ================");

        await _logger.LogWarning("--- Phase 1: Teach ---");

        foreach (var teacher in _teacherRepo.GetAll())
        {
            await teacher.TeachAsync();
        }

        await _logger.LogWarning("--- Phase 2: Exam creation ---");

        List<Exam> global_exams = new List<Exam>();
        foreach (var teacher in _teacherRepo.GetAll())
        {
            Exam exam = ExamFactory.CreateExam(teacher.Subject);
            global_exams.Add(exam);
            await _logger.LogInfo($"Giáo viên {teacher.Name} tạo đề thi môn {teacher.Subject} (Độ khó: {exam.Difficult}, Số câu: {exam.QuestionCount})");
        }

        await _logger.LogWarning("--- Phase 3: Học sinh làm bài thi ---");
        foreach (var student in _studentRepo.GetAll())
        {
            foreach (var exam in global_exams)
            {
                student.TakeExam(exam);
            }
        }

        await _logger.LogWarning("--- BƯỚC 4: Tính GPA học sinh ---");
        foreach (var student in _studentRepo.GetAll())
        {
            student.SetGPA();
            await _logger.LogSuccess($"Học sinh {student.Name} | GPA: {student.GPA:F2}");
        }

        await _logger.LogWarning("--- BƯỚC 5: Đánh giá giáo viên & Thưởng/Phạt lương ---");

        await EvaluateAndManageTeachersAsync();

        await _logger.LogSuccess($"================ KẾT THÚC HỌC KỲ {_semesterNumber} ================\n");
        foreach (var student in _studentRepo.GetAll())
        {
            student.ExamResult.Clear();
        }
        foreach (var teacher in _teacherRepo.GetAll())
        {
            teacher.Salary = 6000;
        }
    }

    private async Task EvaluateAndManageTeachersAsync()
    {
        var teachers = new List<Teacher>(_teacherRepo.GetAll());

        foreach (var teacher in teachers)
        {
            var examWithSubject = _studentRepo.GetAllExamResultsBySubject(teacher.Subject);
            if (!examWithSubject.Any()) continue;

            var studentSubjectGrade = examWithSubject.Select(e => e.Grade).ToList();

            decimal avgStudentGPA = _GPACalc.GPACalculate(studentSubjectGrade);
            teacher.EvaluateStudents(avgStudentGPA);

            if (teacher.Salary < _fireThreshold)
            {
                await _logger.LogError($"[CẢNH BÁO] Giáo viên {teacher.Name} (Môn {teacher.Subject}) có lương {teacher.Salary} < {_fireThreshold} -> ĐÃ NGHỈ VIỆC!");

                // Xóa giáo viên cũ
                _teacherRepo.Remove(teacher);
                // Tuyển giáo viên mới cùng môn học (FR08)
                Teacher newTeacher = TeacherFactory.CreateRandomTeacher(_logger, teacher.Subject);
                _teacherRepo.Add(newTeacher);
                await _logger.LogSuccess($"[TUYỂN DỤNG] Giáo viên mới {newTeacher.Name} đã gia nhập trường dạy môn {newTeacher.Subject}.");

            }
            else
            {
                await _logger.LogInfo($"Teacher {teacher.Name} finished job with {teacher.Salary} total salary.");
            }
        }

    }

}