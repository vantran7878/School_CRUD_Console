
using System.Reflection.Metadata;
using System.Security.Cryptography;
using School_CRUD_console.Interfaces;

namespace School_CRUD_console.Models;

public abstract class Person
{
    private string _name = string.Empty;
    private int _age;

    protected static int max_age;

    static Person() => max_age = 100;
    private string _persionID = string.Empty;

    protected ILogger _logger;

    // public Person(string name, int age, string persionID) => (_name, _age, _persionID) = (name, age, persionID);
    public Person(string name, int age, string persionID, ILogger logger)
    {
        (_name, _age, _persionID, _logger) = (name, age, persionID, logger);
    }

    public string Name
    {
        get => _name; set => _name = value;
    }
    public int Age
    {
        get => _age;
        set
        {
            if (value < 0)
                throw new ArgumentException("Age cannot be negative");

            if (value > max_age)
                throw new ArgumentException("Age cannot higher than max age");
            _age = value;
        }
    }
    public string PersionID
    {
        get => _persionID; set => _persionID = value;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine("\n----- Thong tin nguoi dung -----");
        Console.WriteLine($"Ho & Ten: {_name}, {_age} tuoi");
    }
}


