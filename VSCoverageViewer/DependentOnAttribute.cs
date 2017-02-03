using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    sealed class DependentOnAttribute : Attribute
    {
        public string ParentProperty { get; }

        public DependentOnAttribute(string parentPropertyName)
        {
            ParentProperty = parentPropertyName;
        }
    }
}
