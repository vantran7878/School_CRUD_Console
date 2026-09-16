namespace School_CRUD_console.Interfaces;
using School_CRUD_console.Model;
public interface IPersonService
{
    void Add(Person person);
    void Remove(Person person);
    Person? FindByName(string name);
    Person? FindByID(string id);

    void DisplayAll();
}