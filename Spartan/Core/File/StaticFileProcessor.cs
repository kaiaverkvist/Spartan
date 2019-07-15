using System;
using System.IO;
using System.Net;
using Spartan.Core.Http;

namespace Spartan.Core.File
{
    /// <summary>
    /// This class first ensures a _Public directory exists, and also serves the static files in the folder.
    /// </summary>
    public class StaticFileProcessor
    {
        public StaticFileProcessor()
        {
            #region Public directory setup
            // If we don't have a "_Public" directory, create it.
            if (!Directory.Exists("_Public"))
            {
                Console.WriteLine("Public file directory does not exist. Creating it.");

                // Create the directory.
                Directory.CreateDirectory("_Public");
            }
            #endregion
        }

        /// <summary>
        /// Processes file requests to the _Public directory.
        /// </summary>
        /// <param name="errorHandler"></param>
        /// <param name="context"></param>
        public void Process(IErrorHandler errorHandler, HttpListenerContext context)
        {
            // Get a filename from the url.
            string filename = context.Request.Url.AbsolutePath;

            // Replace the "browser filename" with a realistic filename that accesses the public directory.
            string realisticFilename = Directory.GetCurrentDirectory() +
                                       filename
                                       .Replace("public", "_Public")
                                       .Replace("static", "_Public");

            // If the file exists, we can serve it.
            if (System.IO.File.Exists(realisticFilename))
            {
                try
                {
                    Stream input = new FileStream(realisticFilename, FileMode.Open);

                    // Set the content type and length header.
                    context.Response.ContentType = MimeTypeConverter.GetMimeTypeByExtension(realisticFilename);
                    context.Response.ContentLength64 = input.Length;

                    // Add data about file and current time
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(realisticFilename).ToString("r"));

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    
                    byte[] buffer = new byte[1024 * 32];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();
                    context.Response.OutputStream.Flush();

                }
                catch (Exception ex)
                {
                    errorHandler.HandleError(exception: ex);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

                context.Response.OutputStream.Close();
            }
            else
            {
                // If the file exists, we should return a 404 response.
                HttpResponseWriter.WriteResponse(errorHandler, context, HttpStatusCode.NotFound);
            }
        }
    }
}