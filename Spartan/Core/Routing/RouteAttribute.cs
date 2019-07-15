using System;

namespace Spartan.Core.Routing
{
    /// <summary>
    /// The Route attribute is given to a View class.
    /// Route Manager uses this attribute to find views.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RouteAttribute : Attribute
    {
        public string Path { get; set; }

        public RouteAttribute(string path)
        {
            this.Path = path;
        }
    }
}