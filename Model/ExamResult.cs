namespace School_CRUD_console.Models;


public class ExamResult
{
    private Exam _exam;
    private decimal _grade;

    public decimal Grade
    {
        get => _grade;
        set => _grade = value;
    }

    public Exam exam
    {
        get => _exam;
        set => _exam = value;
    }
}