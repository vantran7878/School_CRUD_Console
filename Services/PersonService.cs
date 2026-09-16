namespace School_CRUD_console.Services;
using School_CRUD_console.Interfaces;
using School_CRUD_console.Model;

public class PersonService : IPersonService
{
    private List<Person> list;

    public PersonService()
    {
        list = new List<Person>();
    }
    
    public void Add(Person p)
    {
        list.Add(p);
        Console.WriteLine("A new person has been added");
        Console.WriteLine("New person info");
        p.DisplayInfo();
    }

    public void Remove(Person p)
    {
        if (!list.Contains(p))
        {
            Console.WriteLine("Khong co user trong list");
            return;
        }  
        Console.WriteLine($"Da xoa thanh cong user {p.Name}");
    }
    public Person FindByName(string ID)
    {
        Person found_person = list.Find(u => u.PersionID == ID)!;
        if (found_person == null)
        {
            Console.WriteLine("Khong co user trong list, khong tim thay duoc");
            return null!;
        }  
        Console.WriteLine($"Tim duoc user {found_person.Name}");

        return found_person;

    }

    public Person FindByID(string ID)
    {
        Person found_person = list.Find(u => u.PersionID == ID)!;
        if (found_person == null)
        {
            Console.WriteLine("Khong co user trong list, khong tim thay duoc");
            return null!;
        }  
        Console.WriteLine($"Tim duoc user {found_person.Name}");

        return found_person;

    }

    public void DisplayAll()
    {
        Console.WriteLine("*** User in the list ***");
        foreach (Person p in list)
        {
            Console.WriteLine("%%%%%");
            p.DisplayInfo();
        }
    }
}