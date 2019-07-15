using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Spartan.Core.File;
using Spartan.Core.Http;
using Spartan.Core.Templating;
using Spartan.Core.View;

namespace Spartan.Core.Routing
{
    /// <summary>
    /// The Route management class which directs requests to the right views.
    /// </summary>
    public class RouteManager
    {
        #region Error handling
        private IErrorHandler _errorHandler;
        #endregion

        #region Views list
        public Dictionary<string, IView> Views;
        #endregion

        // Store the Static file processor
        private readonly StaticFileProcessor _staticFileProcessor;

        // Store the templating processor
        private readonly TemplatingProcessor _templatingProcessor;

        /// <summary>
        /// Initializes the route management class.
        /// This will also add all IView derived classes containing a RouteAttribute to a list.
        /// </summary>
        /// <param name="errorHandler"></param>
        public RouteManager(IErrorHandler errorHandler)
        {
            // Initialize the error handler.
            _errorHandler = errorHandler;

            // Create a list of views
            InitializeViews();

            // Create an instance of the static file processor
            _staticFileProcessor = new StaticFileProcessor();

            // Create an instance of the templating processor
            _templatingProcessor = new TemplatingProcessor(_errorHandler);
        }

        /// <summary>
        /// Finds all the views with a Route attribute.
        /// </summary>
        private void InitializeViews()
        {
            // Initialize the View list.
            Views = new Dictionary<string, IView>();

            #region Find views using reflection
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types in our assembly.
                foreach (Type type in assembly.GetTypes())
                {
                    // Get all RouteAttributes.
                    var attributes = (RouteAttribute[])type.GetCustomAttributes(typeof(RouteAttribute), false);

                    // If we find a RouteAttribute for the given type object.
                    if (attributes.Length > 0)
                    {
                        // For every one of the RouteAttributes, add it to the View list.
                        foreach (var routeAttribute in attributes)
                        {
                            // Create an instance of the type we can add to the list.
                            var t = (IView)Activator.CreateInstance(type);

                            // Add the IView instance to the list along with the path.
                            Views.Add(routeAttribute.Path, t);
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Returns the route path from the Url segments.
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        string GetSegmentPath(string[] segments)
        {
            string segmentOutput = "";
            for (int i = 0; i < segments.Length; i++)
            {
                segmentOutput += segments[i];
            }

            return segmentOutput;
        }

        /// <summary>
        /// Returns a view from a path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>IView object</returns>
        IView GetViewFromPath(string path)
        {
            // Get the first IView where our given path matches the path of the IView.
            return Views.FirstOrDefault(p => p.Key == path).Value;
        }

        /// <summary>
        /// Writes a response by getting data from a view if found. Uses the context to determine request.
        /// </summary>
        /// <param name="context"></param>
        public void ReceiveRequest(HttpListenerContext context)
        {
            string methodName = GetSegmentPath(context.Request.Url.Segments);

            // TODO: Add static file directory

            // If we use a /public/ route, serve files instead of views.
            if (methodName.StartsWith("/public"))
            {
                _staticFileProcessor.Process(_errorHandler, context);
            }
            else
            {
                // Get a view from the View dictionary.
                IView view = GetViewFromPath(methodName);

                // If we find a view, write the data from the view.
                if (view != null)
                {
                    // Get the view, and pass the context in along with it.
                    string returnString = view.GetView(context, _templatingProcessor);

                    HttpResponseWriter.WriteResponse(_errorHandler, context, returnString);
                }
                else // Else, write the 404 response.
                {
                    HttpResponseWriter.WriteResponse(_errorHandler, context, HttpStatusCode.NotFound);
                }
            }
        }

    }
}