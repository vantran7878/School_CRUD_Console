using School_CRUD_console.Interface;
namespace School_CRUD_console.SalaryPolicy;


public class NormalSalaryPolicy(int _threshold)
{
    private readonly string? _status;

    public int ThreshHold { get; set; } = _threshold;

    public void SalaryCalculate(double averageStudentGPA)
    {

    }

}
