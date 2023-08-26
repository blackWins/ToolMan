using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scriban;
using Scriban.Runtime;
using Volo.Abp.DependencyInjection;

namespace Toolkit.Generator
{
    public class TemplateRender : ITransientDependency
    {
        public virtual async Task<string> RenderFromFileAsync(
            [NotNull] string templateFullPath,
            [CanBeNull] object model = null,
            [CanBeNull] Dictionary<string, object> globalContext = null)
        {
            var templateContent = File.ReadAllText(templateFullPath);

            return await RenderAsync(templateContent, model, globalContext, templateFullPath);
        }

        public virtual async Task<string> RenderAsync(
            [NotNull] string templateContent,
            [CanBeNull] object model = null,
            [CanBeNull] Dictionary<string, object> globalContext = null,
            [CanBeNull] string? includeTemplatePath = null)
        {
            if (globalContext == null)
            {
                globalContext = new Dictionary<string, object>();
            }

            var context = CreateScribanTemplateContext(globalContext, model);

            if (!templateContent.IsNullOrWhiteSpace())
            {
                context.TemplateLoader = new TemplateLoader(Path.HasExtension(includeTemplatePath) ? Path.GetDirectoryName(includeTemplatePath) : includeTemplatePath!);
            }

            return (await Template
                    .Parse(templateContent)
                    .RenderAsync(context)).Replace("\r\n", Environment.NewLine);
        }

        protected virtual TemplateContext CreateScribanTemplateContext(Dictionary<string, object> globalContext, object model = null)
        {
            var context = new TemplateContext();

            var scriptObject = new ScriptObject();

            scriptObject.Import(globalContext);

            if (model != null)
            {
                scriptObject.Import(model, renamer: member => member.Name);
                context.MemberRenamer = member => member.Name;
            }

            context.PushGlobal(scriptObject);
            context.PushCulture(System.Globalization.CultureInfo.CurrentCulture);

            return context;
        }
    }
}