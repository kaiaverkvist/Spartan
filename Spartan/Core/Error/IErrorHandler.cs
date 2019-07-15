using System;

namespace Spartan.Core
{
    /// <summary>
    /// The IErrorHandler interface ensures Spartan can send errors to the right place.
    /// </summary>
    public interface IErrorHandler
    {
        void HandleError(string message = null, Exception exception = null);
    }
}