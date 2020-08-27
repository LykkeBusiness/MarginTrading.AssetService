using System;
using System.ComponentModel;

namespace MarginTrading.AssetService.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum enumObj)
            where TEnum : struct, IConvertible
        {
            var enumType = enumObj.GetType();

            if (!enumType.IsEnum) throw new ArgumentException("TEnum must be an enumerated type");

            var fieldInfo = enumType.GetField(enumObj.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttributes.Length == 0)
            {
                return enumObj.ToString();
            }
            return ((DescriptionAttribute)descriptionAttributes[0]).Description;
        }
    }
}
