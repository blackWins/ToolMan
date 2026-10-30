using Scriban.Runtime;
using System;
using System.Linq;

namespace Toolkit.Generator
{
    public class ToolScriptObject : ScriptObject
    {
        public static string KebabCase(string text)
        {
            if (text.IsNullOrWhiteSpace()) return text;
            var parts = text.Split('.').Select(part => part.ToKebabCase());
            return string.Join('.', parts);
        }

        public static string CamelCase(string text)
        {
            if (text.IsNullOrWhiteSpace()) return text;
            var parts = text.Split('.').Select(part => part.ToCamelCase());
            return string.Join('.', parts);
        }

        public static string JoinAsString(ScriptArray array, string separator)
        {
            return string.Join(separator, array.Where(x => x != null));
        }
    }
}