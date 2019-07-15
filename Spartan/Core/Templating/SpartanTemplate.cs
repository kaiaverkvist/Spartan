using Scriban;
using Spartan.Core.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Spartan.Core.Templating
{
    /// <summary>
    /// This class contains data for a given template.
    /// </summary>
    public class SpartanTemplate
    {
        // Keep a string storing our source code
        private string _sourceText;
        
        // Store a reference to the processor.
        private TemplatingProcessor _templatingProcessor;

        /// <summary>
        /// Contains the template's variables
        /// </summary>
        public IDictionary<string, object> TemplateContext;

        /// <summary>
        /// Spawns a template instance containing only the loaded file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="templatingProcessor"></param>
        /// <param name="view"></param>
        public SpartanTemplate(System.IO.FileInfo file, TemplatingProcessor templatingProcessor,
            IView view)
        {
            // Load and prepare our template.
            Load(templatingProcessor, file, view);
        }

        /// <summary>
        /// Gives an instance of SpartanTemplate with a content variable already included.
        /// </summary>
        /// <param name="basePagePath"></param>
        /// <param name="contentPagePath"></param>
        /// <param name="templatingProcessor"></param>
        /// <param name="view"></param>
        public SpartanTemplate(string basePagePath, string contentPagePath, TemplatingProcessor templatingProcessor,
            IView view)
        {
            // Define the FileInfo objects we will later use to read the templates.
            FileInfo basePageFile = new FileInfo($"_Templates/{basePagePath}");
            FileInfo contentPageFile = new FileInfo($"_Templates/{contentPagePath}");

            // Load using the base page first
            Load(templatingProcessor, basePageFile, view);

            #region Setup content
            // Define a new template
            SpartanTemplate contentTemplate = new SpartanTemplate(contentPageFile, templatingProcessor, view);
            TemplateContext.Add("content", contentTemplate.Render());
            #endregion
        }

        /// <summary>
        /// Renders the template using the given template processor.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            // Render the template using the processor
            return _templatingProcessor.TransformTemplate(_sourceText, this);
        }

        /// <summary>
        /// Returns a dictionary containing a list of parameters with the [Replicate] attribute on them.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetParametersFromReflection(IView view)
        {
            // Create a new dictionary to hold our return result.
            Dictionary<string, object> returnAttributes = new Dictionary<string, object>();

            #region Find replicated params using reflection
            foreach (FieldInfo property in view.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                object[] attrs = property.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    ReplicateAttribute isReplicated = attr as ReplicateAttribute;
                    if (isReplicated != null)
                    {
                        string propName = property.Name;


                        returnAttributes.Add(propName, property.GetValue(view));
                    }
                }
            }
            #endregion

            // Finally return the result.
            return returnAttributes;
        }

        /// <summary>
        /// Loads the data required for a basic template constructor.
        /// Internal use for SpartanTemplate class.
        /// </summary>
        /// <param name="templatingProcessor"></param>
        /// <param name="file"></param>
        /// <param name="view"></param>
        private void Load(TemplatingProcessor templatingProcessor, FileInfo file, IView view)
        {
            // Set the processor.
            this._templatingProcessor = templatingProcessor;

            // Create a temporary dictionary to hold the reflected parameters.
            IDictionary<string, object> templateParameters = new Dictionary<string, object>();

            foreach (var contextEntry in GetParametersFromReflection(view))
            {
                // This if check prevents our template system adding duplicates if we inherit multiple templates.
                if(!templateParameters.ContainsKey(contextEntry.Key))
                    templateParameters.Add(contextEntry);
            }

            // Pass in the arguments.
            this.TemplateContext = templateParameters;

            try
            {
                // Open the file and read the source from it.
                using (StreamReader sr = file.OpenText())
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        this._sourceText = this._sourceText + s;
                    }
                }
            }
            catch (Exception ex)
            {
                _templatingProcessor.HandleReadError(ex);
            }
        }
    }
}