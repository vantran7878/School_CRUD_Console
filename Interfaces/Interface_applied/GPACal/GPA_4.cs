using School_CRUD_console.Interfaces;
namespace School_CRUD_console.GPACalc;

public class GPA4Calculator : IGPACalculator
{
    public decimal GPACalculate(List<decimal> gradesList)
    {
        decimal GPA = 0;
        foreach (var grade in gradesList)
        {
            decimal cur_GPA = grade
            switch
            {
                >= 90 and <= 100 => 4.0m,
                >= 80 and < 90 => 3.5m,
                >= 70 and < 80 => 3.0m,
                >= 60 and < 70 => 2.5m,
                >= 50 and < 60 => 2.0m,
                _ => 0.0m
            };
            GPA += cur_GPA;
        }
        return GPA / gradesList.Count();
    }
}