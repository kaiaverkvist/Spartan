using System;

namespace Spartan.Core.Error
{
    /// <summary>
    /// The DefaultErrorHandler class handles errors from Exceptions, etc.
    /// </summary>
    public class DefaultErrorHandler : IErrorHandler
    {
        public void HandleError(string message = null, Exception exception = null)
        {
            // If we receive an error, we print the message or Exception data to console.
            if (message != null)
            {
                Console.WriteLine(message);
            }

            if (exception != null)
            {
                Console.WriteLine(exception);
            }
        }
    }
}