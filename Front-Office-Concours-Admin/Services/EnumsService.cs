using System;
using System.ComponentModel;
using System.Reflection;

namespace Front_Office_Concours_Admin.Services
{
    public class EnumsService
    {
        public string GetDescriptionFromEnumValue(Type enumType, int value)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("Le type doit être un enum");

            var enumValue = Enum.ToObject(enumType, value);

            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();

            return attribute != null ? attribute.Description : enumValue.ToString();
        }
    }
}