using System.Collections.Generic;
using System.Net;
using Spartan.Core.Templating;

namespace Spartan.Core.View
{
    /// <summary>
    /// An instance of the IView class is created upon server initialization which is used during the entire lifetime of the server.
    /// </summary>
    public interface IView
    {
        string GetView(HttpListenerContext context, TemplatingProcessor templatingProcessor);
    }
}