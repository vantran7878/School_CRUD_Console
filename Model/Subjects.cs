using System.ComponentModel;
using System.Reflection;
namespace School_CRUD_console.Subjects;

public enum Subject
{
    [Description("Mathematics")]
    Math,
    [Description("Physics")]
    Physics,
    [Description("Chemistry")]
    Chemistry,
    [Description("Biology")]
    Biology,
    [Description("Literature")]
    Literature,
    [Description("History")]
    History,
    [Description("Geography")]
    Geography,
    [Description("English")]
    English,
}

public static class EnumHelper
{
    /// <summary>
    /// Chuyển chuỗi (tên Enum hoặc Description) thành giá trị Enum tương ứng.
    /// </summary>
    public static T ParseFromDescription<T>(string description) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description));

        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            
            // So sánh với Description nếu có
            if (attribute != null && string.Equals(attribute.Description, description, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null)!;
            }

            // So sánh với Tên của Enum
            if (string.Equals(field.Name, description, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null)!;
            }
        }

        throw new ArgumentException($"Không tìm thấy Enum {typeof(T).Name} tương ứng với giá trị: {description}");
    }

    /// <summary>
    /// Lấy chuỗi Description của giá trị Enum (nếu không có sẽ trả về Enum.ToString()).
    /// </summary>
    public static string GetDescription<T>(this T enumValue) where T : struct, Enum
    {
        var field = typeof(T).GetField(enumValue.ToString());
        if (field != null)
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
            {
                return attribute.Description;
            }
        }
        return enumValue.ToString();
    }
}

// 1. Chuyển từ Description/String sang Enum
// Subject s1 = EnumHelper.ParseFromDescription<Subject>("Mathematics"); // Subject.Math
// Subject s2 = EnumHelper.ParseFromDescription<Subject>("Math");        // Subject.Math (so sánh theo tên)

// // 2. Chuyển từ Enum sang Description (dùng Extension Method)
// string desc = Subject.Math.GetDescription(); // Trả về "Mathematics"