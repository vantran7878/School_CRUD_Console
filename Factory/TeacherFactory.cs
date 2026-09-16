using School_CRUD_console.Model;
using School_CRUD_console.Interface;
using School_CRUD_console.Subjects;
using School_CRUD_console.RandomExt;


public class TeacherFactory
{

    private static readonly string[] FirstNames = { "Alice", "Bob", "Charlie", "David", "Eva", "Frank", "Grace", "Henry" };
    private static readonly string[] LastNames = { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Vuoung" };

    private static readonly Random Rand = new Random();

    private static int _idCounter = 1;

    public static Teacher CreateRandomTeacher(ILogger logger, Subject subject)
    {
        string firstName = FirstNames[Rand.Next(FirstNames.Length)];
        string lastNames = LastNames[Rand.Next(LastNames.Length)];
        string fullName = $"{lastNames} {firstName}";


        string teacherID = $"TCH-{_idCounter++:D3}";
        //lay 3 chu so

        string personID = $"CCCD-{Rand.Next((int)1e6, (int)1e7)}";

        int age = Rand.Next(28, 59);
        int expYear = Rand.Next(1, 21);
        int defaultSalary = 6000;

        // Subject subject = RandomExtensions.NextEnum<Subject>(Rand);

        Teacher teacher = new Teacher(fullName, age, personID, teacherID, subject, defaultSalary, expYear, logger);

        return teacher;
    }
}