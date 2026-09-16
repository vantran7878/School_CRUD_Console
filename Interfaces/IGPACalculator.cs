using School_CRUD_console.Model;
namespace School_CRUD_console.Interface;

public interface IGPACalculator
{
    decimal GPACalculate(List<decimal> gradeList);
}