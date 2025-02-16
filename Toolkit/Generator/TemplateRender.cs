using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Runtime;
using Volo.Abp.DependencyInjection;

namespace Toolkit.Generator
{
    public class TemplateRender : ITransientDependency
    {
        public virtual async Task<string> RenderFromFileAsync(
            [NotNull] string templateFullPath,
            [CanBeNull] JObject? model = null,
            [CanBeNull] Dictionary<string, object>? globalContext = null,
            [CanBeNull] string? includeTemplatePath = null)
        {
            var templateContent = File.ReadAllText(templateFullPath);

            return await RenderAsync(templateContent, model, globalContext, includeTemplatePath ?? templateFullPath);
        }

        public virtual async Task<string> RenderAsync(
            [NotNull] string template,
            [CanBeNull] JObject? model = null,
            [CanBeNull] Dictionary<string, object>? globalContext = null,
            [CanBeNull] string? includeFilePath = null)
        {
            if (globalContext == null)
            {
                globalContext = new Dictionary<string, object>();
            }

            var context = CreateTemplateContext(globalContext, model);

            if (!template.IsNullOrWhiteSpace())
            {
                context.TemplateLoader = new TemplateLoader(Directory.Exists(includeFilePath) ? includeFilePath! : Path.GetDirectoryName(includeFilePath));
            }

            var content = (await Template
                   .Parse(template)
                   .RenderAsync(context)).Replace("\r\n", Environment.NewLine);

            context.CurrentGlobal.TryGetValue(TookitConsts.SKIP_GENERATE, out object value);

            if (value is bool skipGenerate && skipGenerate)
            {
                return string.Empty;
            }

            return content;
        }

        protected virtual TemplateContext CreateTemplateContext(Dictionary<string, object> globalContext, JObject? model = null)
        {
            var context = new TemplateContext();

            var scriptObject = new ScriptObject();

            scriptObject.Import(globalContext);
            scriptObject.SetValue("abp", new AbpScriptObject(), true);

            if (model != null)
            {
                scriptObject.Import(ToScriptObject(model), renamer: member => member.Name);
                context.MemberRenamer = member => member.Name;
            }

            context.PushGlobal(scriptObject);
            context.PushCulture(System.Globalization.CultureInfo.CurrentCulture);

            return context;
        }

        protected virtual ScriptObject ToScriptObject(JObject element)
        {
            var obj = new ScriptObject();
            foreach (var prop in element.Properties())
            {
                if (prop.Value.Type == JTokenType.Object)
                {
                    obj[prop.Name] = ToScriptObject(prop.Value.ToObject<JObject>()!);
                    continue;
                }

                obj[prop.Name] = GetTokenValue(prop.Value);
            }
            return obj;
        }

        protected virtual object GetTokenValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    var array = new ScriptArray();
                    foreach (var nestedElement in token.Children())
                    {
                        array.Add(GetTokenValue(nestedElement));
                    }
                    return array;
                case JTokenType.String:
                    return token.ToString();
                case JTokenType.Integer:
                    return token.ToObject<int>();
                case JTokenType.Float:
                    return token.ToObject<double>();
                case JTokenType.Boolean:
                    return token.ToObject<bool>();
                case JTokenType.Date:
                    return token.ToObject<DateTime>();
                case JTokenType.Undefined:
                case JTokenType.Null:
                    return null;
                default:
                    return token;
            }
        }

    }
}