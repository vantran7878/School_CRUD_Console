namespace School_CRUD_console.Model;

using School_CRUD_console.Logger;
using School_CRUD_console.Constant;
using School_CRUD_console.Enums;
using School_CRUD_console.Interface;

public class Student : Person
{
    private string _studentid = string.Empty;
    private decimal _GPA;

    private string _major = string.Empty;
    private List<ExamResult> _examResults = new();

    public IList<ExamResult> ExamResult => _examResults;

    private static readonly Random _rand = new Random();

    protected IGPACalculator _GPAcalc;

    public Student(string name, int age, string ID, string studentID, decimal gpa, string major, ILogger logger, IGPACalculator gpaCalculator) : base(name, age, ID, logger)
    {
        (_studentid, _GPA, _major, _logger, _GPAcalc) = (studentID, gpa, major, logger, gpaCalculator);
    }

    public string StudentID
    {
        get => _studentid;
        set => _studentid = value;
    }

    public decimal GPA
    {
        get => _GPA; set => _GPA = value;
    }

    public string Major
    {
        get => _major;
        set => _major = value;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine("--- Thong tin sinh vien:");
        Console.WriteLine($"- Ma so sinh vien: {StudentID}");
        Console.WriteLine($"- Sinh vien nam {Age - 17}");
        Console.WriteLine($"- Chuyen nganh {Major}");
        Console.WriteLine($"- GPA: {GPA}");
    }

    public void TakeExam(Exam exam)
    {
        int grade = exam.Difficult switch
        {
            ExamDifficult.Easy => _rand.Next(SchoolConstants.LOW_EASY_DIFF, SchoolConstants.HIGH_EASY_DIFF + 1),
            ExamDifficult.Medium => _rand.Next(SchoolConstants.LOW_MEDIUM_DIFF, SchoolConstants.HIGH_MEDIUM_DIFF + 1),
            ExamDifficult.Hard => _rand.Next(SchoolConstants.LOW_HARD_DIFF, SchoolConstants.HIGH_HARD_DIFF + 1),
            _ => _rand.Next(0, 101)

        };

        ExamResult.Add(new ExamResult { exam = exam, Grade = grade });

        // _logger.LogInfo($"Sinh vien {Name} da thi mon {exam.Subject} va dat diem {grade}");
    }

    public void SetGPA()
    {
        List<decimal> gradesList = ExamResult.Select(g => g.Grade).ToList();

        this.GPA = _GPAcalc.GPACalculate(gradesList);
    }
}