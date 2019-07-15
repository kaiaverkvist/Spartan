using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spartan.Core.Templating
{
    /// <summary>
    /// This class ensures there is a _Templates folder, and manages the rendering of templates.
    /// </summary>
    public class TemplatingProcessor
    {
        private readonly IErrorHandler _errorHandler;

        static readonly Regex TokenParserRegex = new Regex(@"{{([^}]+)}}", RegexOptions.CultureInvariant);

        public TemplatingProcessor(IErrorHandler errorHandler)
        {
            #region Template directory setup
            // If we don't have a "_Public" directory, create it.
            if (!Directory.Exists("_Templates"))
            {
                Console.WriteLine("Template file directory does not exist. Creating it.");

                // Create the directory.
                Directory.CreateDirectory("_Templates");
            }
            #endregion

            _errorHandler = errorHandler;
        }

        /// <summary>
        /// Handles an exception from the Template function
        /// </summary>
        /// <param name="exception"></param>
        public void HandleReadError(Exception exception)
        {
            _errorHandler.HandleError(exception: exception);
        }

        /// <summary>
        /// Transforms the template from plain source and inserts variables.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="callerTemplate"></param>
        /// <returns></returns>
        public string TransformTemplate(string input, SpartanTemplate callerTemplate)
        {

            // Create a template instance and populate it with data.
            Template template = Template.Parse(input);

            var context = new TemplateContext() { TemplateLoader = new DefaultTemplateLoader() };

            var tempParams = new ScriptObject();

            foreach (var pair in callerTemplate.TemplateContext)
            {
                tempParams.Add(pair.Key, pair.Value);
            }

            // TODO: Clean this up!
            context.PushGlobal(tempParams);

            string result = template.Render(context);

            // Return the string.
            return result;
        }
    }
}