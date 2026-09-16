namespace School_CRUD_console.Repository;

using System.ComponentModel;
using System.Runtime.InteropServices;
using School_CRUD_console.Model;
using School_CRUD_console.Subjects;

public class StudentRepository
{
    private List<Student> list_student = new();

    public List<Student> GetAll()
    {
        return list_student;
    }

    public Student GetByName(string name)
    {
        Student? t_result = list_student.Find(t => t.Name == name);
        if (t_result != null) return t_result;
        return null;
    }

    public Student GetByStudentID(string studentID)
    {
        Student? t_result = list_student.Find(t => t.StudentID == studentID);
        if (t_result != null) return t_result;
        return null;
    }

    public void Add(Student student)
    {
        list_student.Add(student);
    }

    public void Remove(Student student)
    {
        list_student.Remove(student);
    }

    public List<ExamResult> GetAllExamResultsBySubject(Subject subject)
    {
        List<ExamResult> list_result = new();

        foreach (var student in list_student)
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