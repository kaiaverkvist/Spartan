using System;
using System.Globalization;
using System.Net;
using Spartan.Core.Routing;
using Spartan.Core.Templating;
using Spartan.Core.View;

namespace Spartan.Dev.Views
{
    [Route("/")]
    public class ImageView : IView
    {
        [Replicate]
        public string Test = "sadsadsad";

        [Replicate]
        public string Image;

        [Replicate]
        public string CurrentTime;

        public string GetView(HttpListenerContext context, TemplatingProcessor templatingProcessor)
        {

            CurrentTime = DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
            Image = "/public/fahrenheit-451.png";

            Test = context.Request.RemoteEndPoint.ToString();

            SpartanTemplate template = new SpartanTemplate("image.html", "test.html", templatingProcessor, this);

            // Render the template.
            return template.Render();
        }
    }
}