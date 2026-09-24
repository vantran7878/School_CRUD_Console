namespace School_CRUD_console.Factory;

using School_CRUD_console.Models;
using School_CRUD_console.Enums;
using School_CRUD_console.Subjects;

public static class ExamFactory
{
    private static readonly Random Rand = new Random();

    public static Exam CreateExam(Subject subject)
    {
        // Random độ khó: Easy (0), Medium (1), Hard (2)
        ExamDifficult difficulty = (ExamDifficult)Rand.Next(0, (int)ExamDifficult._Count_ExamDifficult);

        // Random số lượng câu hỏi từ 10 đến 20 câu
        int questionCount = Rand.Next(10, 21);

        // Tạo đề thi hoàn chỉnh
        Exam exam = new Exam(subject)
        {
            Difficult = difficulty,
            QuestionCount = questionCount
        };

        return exam;
    }
}
