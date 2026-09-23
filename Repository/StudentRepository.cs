namespace School_CRUD_console.Repository;

using System.ComponentModel;
using System.Runtime.InteropServices;
using School_CRUD_console.Models;
using School_CRUD_console.Subjects;

public class StudentRepository : InMemoryRepository<Student>
{


    public Student GetByName(string name)
    {
        Student? t_result = _data.Find(t => t.Name == name);
        if (t_result != null) return t_result;
        return null;
    }

    public Student GetByStudentID(string studentID)
    {
        Student? t_result = _data.Find(t => t.StudentID == studentID);
        if (t_result != null) return t_result;
        return null;
    }

    public List<ExamResult> GetAllExamResultsBySubject(Subject subject)
    {
        List<ExamResult> list_result = new();

        foreach (var student in _data)
        {
            foreach (var exam_res in student.ExamResult)
            {
                if (exam_res.exam.Subject == subject)
                {
                    list_result.Add(exam_res);
                }
            }
        }

        return list_result;
    }
}