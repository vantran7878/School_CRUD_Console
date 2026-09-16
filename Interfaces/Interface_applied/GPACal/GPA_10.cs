using School_CRUD_console.Interface;
namespace School_CRUD_console.GPACalc;

public class GPA10Calculator : IGPACalculator
{
    public decimal GPACalculate(List<decimal> gradesList)
    {
        decimal GPA = 0;
        foreach (var grade in gradesList)
        {
            GPA += grade;
        }
        return GPA / gradesList.Count();
    }
}