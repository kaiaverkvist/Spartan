using System;
using System.Net;
using System.Threading;
using Spartan.Core.Http;
using System.IO;
using System.Net.Sockets;

namespace Spartan.Core
{
    /// <summary>
    /// This acts as the entry point for the framework.
    /// This class spawns all the necessary instances for the web server.
    /// 
    /// </summary>
    public class SpartanServer
    {
        #region HttpListener
        public readonly HttpListener Listener = new HttpListener();
        #endregion

        #region Error handler instantiation

        public readonly IErrorHandler ErrorHandler;
        public readonly HttpHandler HttpHandler;
        #endregion

        /// <summary>
        /// Spawns a SpartanServer instance.
        /// </summary>
        /// <param name="spartanConfiguration">SpartanConfiguration</param>
        /// <param name="port">the http listening port</param>
        public SpartanServer(SpartanConfiguration spartanConfiguration, int port = 0)
        {
            #region Error handler setup
            // Set the default error handler.
            this.ErrorHandler = spartanConfiguration.GetErrorHandler();
            #endregion

            #region Http handler setup
            // Set the default http handler.
            this.HttpHandler = new HttpHandler(ErrorHandler);
            #endregion

            // Check that we can use the HttpListener class.
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("System does not support HttpListener.");

            
            // Add a generic prefix to serve requests. This is required by the HttpListener class.
            // Use the port given using the parameter.

            string portString = "/";
            if (port != 0)
            {
                portString = $":{port}/";
            }

            Listener.Prefixes.Add($"http://*{portString}");

            try
            {
                Listener.Start();
            }
            catch (HttpListenerException ex)
            {
                ErrorHandler.HandleError("", ex);
            }
            catch (SocketException ex)
            {
                ErrorHandler.HandleError("Port may be blocked, or system may be denying HTTP socket access.", ex);
            }
            catch (AccessViolationException ex)
            {
                ErrorHandler.HandleError(
                    "Some systems may be blocking the HTTP listener from using the port. Try running as administrator.",
                    ex);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError("Unknown exception.", ex);
            }
        }

        /// <summary>
        /// Runs the SpartanServer HttpListener.
        /// This should be called once to create a listening loop.
        /// Use .Stop() when done.
        /// </summary>
        public void Run()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    while (Listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(contextObject =>
                        {
                            // Validate the context.
                            if (contextObject is HttpListenerContext context)
                            {
                                // Send the request context to the HttpHandler.
                                HttpHandler.Receive(context);
                            }
                            else
                            {
                                ErrorHandler.HandleError("Context object was invalid during handler processing.");
                            }
                        }, Listener.GetContext());
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleError(exception: ex);
                }
            });
        }

        /// <summary>
        /// Stops the SpartanServer HttpListener.
        /// </summary>
        public void Stop()
        {
            Listener.Stop();
            Listener.Close();
        }
    }
}