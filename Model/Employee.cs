using School_CRUD_console.Interface;

namespace School_CRUD_console.Model;

public class Employee : Person
{
    private string _department;
    private int _salary;

    public string Department
    {
        get => _department;
        set => _department = value;
    }

    public int Salary
    {
        get => _salary;
        set => _salary = value;
    }

    public Employee(string name, int age, string persionID, string department, int salary, ILogger logger) : base(name, age, persionID, logger)
    {
        _department = department;
        _salary = salary;
    }

    public int GetIncome()
    {
        return Salary;
    }
}