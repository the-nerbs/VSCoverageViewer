using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// Indicates that the specified parent property affects the value of the property this
    /// attribute is applied to. Any property change notifications for the parent property should
    /// trigger property change notifications for this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    internal sealed class DependentOnAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the parent property.
        /// </summary>
        public string ParentProperty { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="DependentOnAttribute"/>.
        /// </summary>
        /// <param name="parentPropertyName">The name of the parent property.</param>
        public DependentOnAttribute(string parentPropertyName)
        {
            ParentProperty = parentPropertyName;
        }
    }
}
