using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VSCoverageViewer.Serialization
{
    public class MethodCoverageInfo
    {
        public string MethodKeyName { get; set; }
        public string MethodName { get; set; }
        public string MethodFullName { get; set; }

        public int LinesCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }
        public int LinesNotCovered { get; set; }

        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }

        [XmlElement("Lines")]
        public LineCoverageInfo[] Lines { get; set; }


        internal MethodCoverageInfo Clone()
        {
            var clone = (MethodCoverageInfo)MemberwiseClone();

            clone.Lines = Lines.Select(l => l.Clone()).ToArray();

            return clone;
        }
    }
}
