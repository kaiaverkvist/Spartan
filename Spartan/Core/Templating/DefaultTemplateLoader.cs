using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spartan
{
        
    public class DefaultTemplateLoader : ITemplateLoader
    {
        string ITemplateLoader.GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(Environment.CurrentDirectory, templateName);
        }

        string ITemplateLoader.Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            // Template path was produced by the `GetPath` method above in case the Template has 
            // not been loaded yet
            // TODO: Fix path loading
            string path = Path.GetFileName(templatePath);
            string dir = Path.GetDirectoryName(templatePath);

            string result;

            try
            {
                result = File.ReadAllText(Path.Combine(dir, "_Templates", path));
            } catch(Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
