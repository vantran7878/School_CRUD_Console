namespace School_CRUD_console.Models;

using System;
using System.Dynamic;
using School_CRUD_console.Enums;
using School_CRUD_console.Subjects;

public class Exam
{
    private int _question_count;
    private ExamDifficult _difficult;

    private Subject _subject;

    public int QuestionCount
    {
        get => _question_count;
        set => _question_count = value;
    }

    public ExamDifficult Difficult
    {
        get => _difficult;
        set => _difficult = value;
    }

    public Subject Subject
    {
        get => _subject;
        set => _subject = value;
    }

    public Exam(Subject subject)
    {
        Random rand = new Random();
        QuestionCount = rand.Next(1, 21);
        Subject = subject;
        Difficult = (ExamDifficult)rand.Next(1, (int)ExamDifficult._Count_ExamDifficult);
    }

    public Exam(Subject subject, ExamDifficult difficult, int questionCount)
    {
        (Subject, Difficult, QuestionCount) = (subject, difficult, questionCount);
    }
}