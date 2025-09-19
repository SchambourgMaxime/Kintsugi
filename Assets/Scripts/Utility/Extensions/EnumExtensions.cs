using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace VV.Utility
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetType().GetTypeInfo().GetMember(value.ToString()).FirstOrDefault(member =>
                member.GetCustomAttributes(typeof(DescriptionAttribute), false).Any());
            return attribute?.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }
    }
}