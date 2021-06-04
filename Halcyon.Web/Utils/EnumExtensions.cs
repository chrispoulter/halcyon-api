using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Halcyon.Web.Utils
{
    public static class EnumExtensions
    {
        public static string GetDiplayName(this Enum value) => value.GetType()
            .GetMember(value.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            .Name;
    }
}
