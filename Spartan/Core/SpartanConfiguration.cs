using Spartan.Core.Error;

namespace Spartan.Core
{
    /// <summary>
    /// The SpartanConfiguration class is passed to the SpartanServer upon creation and determines some parts of how the server handles requests.
    /// If given no IErrorHandler it uses the DefaultErrorHandler class.
    /// </summary>
    public class SpartanConfiguration
    {
        private readonly IErrorHandler _errorHandler;

        public SpartanConfiguration(IErrorHandler defaultErrorHandler = null)
        {
            this._errorHandler = new DefaultErrorHandler();

            if(defaultErrorHandler != null)
            {
                this._errorHandler = defaultErrorHandler;
            }
        }

        public IErrorHandler GetErrorHandler()
        {
            return _errorHandler;
        }
    }
}