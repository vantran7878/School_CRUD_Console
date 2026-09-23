using School_CRUD_console.Models;
namespace School_CRUD_console.Interfaces;

public interface IGPACalculator
{
    decimal GPACalculate(List<decimal> gradeList);
}