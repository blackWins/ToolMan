using System;
using System.Linq;
using System.Reflection;
using Scriban.Runtime;

namespace Toolkit.Generator
{
    public class AbpScriptObject : ScriptObject
    {
        public static string KebabCase(string text)
        {
            var parts = text.Split('.').Select(part => part.ToKebabCase());

            return string.Join('.', parts);
        }

        public static string CamelCase(string text)
        {
            var parts = text.Split('.').Select(part => part.ToCamelCase());

            return string.Join('.', parts);
        }

    }
}