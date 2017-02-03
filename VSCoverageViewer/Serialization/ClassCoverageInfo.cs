using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VSCoverageViewer.Serialization
{
    public class ClassCoverageInfo
    {
        public string ClassKeyName { get; set; }
        public string ClassName { get; set; }

        public int LinesCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }

        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }

        public string NamespaceKeyName { get; set; }


        [XmlElement("Method")]
        public MethodCoverageInfo[] Methods { get; set; }


        internal ClassCoverageInfo Clone()
        {
            var clone = (ClassCoverageInfo)MemberwiseClone();

            clone.Methods = Methods.Select(m => m.Clone()).ToArray();

            return clone;
        }
    }
}
