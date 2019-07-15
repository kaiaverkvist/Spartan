using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spartan.Core.Templating
{
    /// <summary>
    /// The ReplicateAttribute tells Spartan to include a given variable in the page globals list.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReplicateAttribute : Attribute
    {
        public ReplicateAttribute()
        {
        }
    }
}
