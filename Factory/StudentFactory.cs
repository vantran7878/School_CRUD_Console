using School_CRUD_console.Models;
using School_CRUD_console.Interfaces;
using School_CRUD_console.Subjects;
using School_CRUD_console.RandomExt;
using School_CRUD_console.GPACalc;


public class StudentFactory
{

    private static readonly string[] FirstNames = { "Mareli", "Pranav", "Kenzie", "Aiyana", "Dulce", "Karli", "Shaniya", "Reagan", "Ryleigh", "Marilyn", "Jazmyn"
    };
    private static readonly string[] LastNames = { "Henderson", "Rice", "Jacobs", "Jarvis", "Moyer", "Davidson", "Sandoval", "Herman", "Valentine", "Rios", "Ryan", "Roach" };
    private static readonly string[] Majors = { "Ethnicity & Race Studies", "History", "Culinary arts", "Atmospheric physics", "Artificial intelligence", "Philosophical traditions and school" };

    private static readonly Random Rand = new Random();

    private static int _idCounter = 1;

    public static Student CreateRandomStudent(ILogger logger)
    {
        string firstName = FirstNames[Rand.Next(FirstNames.Length)];
        string lastNames = LastNames[Rand.Next(LastNames.Length)];
        string fullName = $"{lastNames} {firstName}";


        string studentID = $"STU-{_idCounter++:D3}";
        //lay 3 chu so

        string personID = $"CCCD-{Rand.Next((int)1e6, (int)1e7)}";

        string major = Majors[Rand.Next(Majors.Length)];

        int age = Rand.Next(28, 59);

        Subject subject = RandomExtensions.NextEnum<Subject>(Rand);
        GPA4Calculator gpaCalc = new();
        Student student = new Student(fullName, age, personID, studentID, 0, major, logger, gpaCalc);

        return student;
    }
}