using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using Spartan.Core.Routing;

namespace Spartan.Core.Http
{
    /// <summary>
    /// The DefaultHttpHandler is an implementation of IHttpHandler that simply sends the request to the routing system.
    /// </summary>
    public class HttpHandler
    {

        #region Events
        public delegate void HttpHandlerEvent(object source, HttpListenerContext context);

        public event HttpHandlerEvent onHttpRequest;
        #endregion

        #region Error handler
        // Initialize the Error Handler.
        private readonly IErrorHandler _errorHandler;
        #endregion

        #region Route manager
        public RouteManager RouteManager;
        #endregion

        /// <summary>
        /// Create a HttpHandler instance with a specified error handler.
        /// </summary>
        /// <param name="errorHandler">error handler</param>
        public HttpHandler(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;

            RouteManager = new RouteManager(_errorHandler);
        }


        /// <summary>
        /// The receive method handles the incoming context object which contains both the Request and Response objects.
        /// </summary>
        /// <param name="context">context object</param>
        public void Receive(HttpListenerContext context)
        {
            // Call the request event
            onHttpRequest?.Invoke(this, context);

            // Send the request to the route manager.
            RouteManager.ReceiveRequest(context);
        }
    }
}