using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VSCoverageViewer.Serialization
{
    public class NamespaceCoverageInfo
    {
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }

        public int LinesCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }

        public string ModuleName { get; set; }
        public string NamespaceKeyName { get; set; }
        public string NamespaceName { get; set; }


        [XmlElement("Class")]
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "Serialization type.")]
        public ClassCoverageInfo[] Classes { get; set; }


        internal NamespaceCoverageInfo Clone()
        {
            var clone = (NamespaceCoverageInfo)MemberwiseClone();

            clone.Classes = Classes.Select(cls => cls.Clone()).ToArray();

            return clone;
        }
    }
}
