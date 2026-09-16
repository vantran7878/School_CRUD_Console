namespace School_CRUD_console.RandomExt;

using School_CRUD_console.Enums;

public static class RandomExtensions
{
    // Hàm random tổng quát cho BẤT KỲ Enum nào
    public static T NextEnum<T>(this Random rand) where T : struct, Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(rand.Next(values.Length))!;
    }
    public static ExamDifficult NextDifficult(this Random rand)
    {
        return (ExamDifficult)rand.Next((int)ExamDifficult.Easy, (int)ExamDifficult._Count_ExamDifficult);
    }
}
