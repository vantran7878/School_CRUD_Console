namespace School_CRUD_console.Repository;

using System.ComponentModel;
using System.Runtime.InteropServices;
using School_CRUD_console.Model;
using School_CRUD_console.Subjects;

public class TeacherRepository
{
    private List<Teacher> list_teacher = new();

    public List<Teacher> GetAll()
    {
        return list_teacher;
    }

    public Teacher GetBySubject(Subject subject)
    {
        Teacher? t_result = list_teacher.Find(t => t.Subject == subject);
        if (t_result != null) return t_result;
        return null;
    }

    public void Add(Teacher teacher)
    {
        list_teacher.Add(teacher);
    }

    public void Remove(Teacher teacher)
    {
        list_teacher.Remove(teacher);
    }
}