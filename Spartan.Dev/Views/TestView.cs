using System.Net;
using Spartan.Core.Routing;
using Spartan.Core.Templating;
using Spartan.Core.View;

namespace Spartan.Dev.Views
{
    [Route("/login")]
    public class TestView : IView
    {
        public string GetView(HttpListenerContext context, TemplatingProcessor templatingProcessor)
        {
            return "View data from TestView";
        }
    }
}