using School_CRUD_console.Subjects;
using School_CRUD_console.Enums;
using School_CRUD_console.RandomExt;
using School_CRUD_console.Interfaces;
using School_CRUD_console.Logger;
namespace School_CRUD_console.Models;

using School_CRUD_console.Factory;
using School_CRUD_console.Interfaces;
using School_CRUD_console.Services;

public class Teacher : Person
{
    private string _teacherID = string.Empty;
    private int _salary;
    private int _expYears;
    private Subject _subject;

    private static readonly Random _rand = new Random();

    public Subject Subject
    {
        get => _subject;
        set => _subject = value;
    }

    public Teacher(string name, int age, string ID, string teacherID, Subject subject, int salary, int expYears, ILogger logger) : base(name, age, ID, logger)
    {
        _subject = subject;
        _teacherID = teacherID;
        _salary = salary;
        _expYears = expYears;
    }

    public string TeacherID
    {
        get => _teacherID;
        set => _teacherID = value;
    }

    public int Salary
    {
        get => _salary;
        set => _salary = value;
    }

    public int ExpYears
    {
        get => _expYears;
        set => _expYears = value;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine("--- Thong tin giang vien:");
        Console.WriteLine($"- Ma so giang vien: {TeacherID}");
        Console.WriteLine($"- Kinh nghiem: {ExpYears} nam");
        Console.WriteLine($"- Luong: {Salary}");
    }

    public int GetIncome()
    {
        return Salary;
    }

    public async Task<Exam> CreateExamAsync()
    {
        Exam new_exam = ExamFactory.CreateExam(this.Subject);

        await _logger.LogSuccess("[EXAM] unit succesfully created");
        await _logger.LogInfo($"[EXAM] unit created with: Subject = {new_exam.Subject}, Difficult = {new_exam.Difficult}, #question = {new_exam.QuestionCount}");

        return new_exam;
    }

    public async Task TeachAsync()
    {
        await _logger.LogSuccess("---Successfully Teach---");
    }

    public void EvaluateStudents(decimal averageStudentGPA)
    {
        Salary += averageStudentGPA switch
        {
            >= 3.8m and <= 4.0m => 1000,
            >= 3.0m and < 3.8m => 500,
            >= 2.5m and < 3.0m => 100,
            >= 2.0m and < 3.5m => -500,
            _ => -1000
        };
    }

}



