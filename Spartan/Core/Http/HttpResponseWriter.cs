using System;
using System.Net;
using System.Text;

namespace Spartan.Core.Http
{
    /// <summary>
    /// A utility class to write http responses.
    /// </summary>
    public static class HttpResponseWriter
    {
        /// <summary>
        /// Sends a response to the client from the given context.
        /// </summary>
        /// <param name="errorHandler"></param>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public static void WriteResponse(IErrorHandler errorHandler, HttpListenerContext context, string data)
        {
            try
            {
                // Create a buffer for our string data.
                byte[] buffer = Encoding.UTF8.GetBytes(data);

                // Write the buffer to the stream.
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(exception: ex);
            }
            finally
            {
                // Close the stream once the receive method is done with it.
                context.Response.OutputStream.Close();
            }
        }

        /// <summary>
        /// Writes a HTTP response but only with a HttpStatusCode.
        /// </summary>
        /// <param name="errorHandler"></param>
        /// <param name="context"></param>
        /// <param name="code"></param>
        public static void WriteResponse(IErrorHandler errorHandler, HttpListenerContext context, HttpStatusCode code)
        {
            try
            {
                context.Response.StatusCode = (int)code;

                // Create a buffer for the 404 message
                byte[] buffer = Encoding.UTF8.GetBytes("404 - No file or route found");

                // Write the 404 message to the stream.
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(exception: ex);
            }
            finally
            {
                // Close the stream once the receive method is done with it.
                context.Response.OutputStream.Close();
            }
        }
    }
}