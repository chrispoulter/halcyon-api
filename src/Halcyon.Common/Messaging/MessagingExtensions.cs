using System.Text.RegularExpressions;

namespace Halcyon.Common.Messaging;

public static class MessagingExtensions
{
    public static string GetQueueName<T>()
    {
        var typeName = typeof(T).FullName;
        return Regex.Replace(typeName.Replace(".", ""), "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
    }
}
