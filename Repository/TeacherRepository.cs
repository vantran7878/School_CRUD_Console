namespace School_CRUD_console.Repository;

using School_CRUD_console.Models;
using School_CRUD_console.Subjects;

public class TeacherRepository : InMemoryRepository<Teacher>
{
    public Teacher GetBySubject(Subject subject)
    {
        Teacher? t_result = _data.Find(t => t.Subject == subject);
        if (t_result != null) return t_result;
        return null;
    }

}