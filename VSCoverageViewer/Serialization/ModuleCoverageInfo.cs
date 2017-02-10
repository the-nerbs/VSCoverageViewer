using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VSCoverageViewer.Serialization
{
    public class ModuleCoverageInfo
    {
        public string ModuleName { get; set; }
        public int ImageSize { get; set; }
        public int ImageLinkTime { get; set; }
        public int LinesCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }

        [XmlElement("NamespaceTable")]
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "Serialization type.")]
        public NamespaceCoverageInfo[] Namespaces { get; set; }


        internal ModuleCoverageInfo Clone()
        {
            var clone = (ModuleCoverageInfo)MemberwiseClone();

            clone.Namespaces = Namespaces.Select(ns => ns.Clone()).ToArray();

            return clone;
        }

        internal void OffsetSourceFileIDs(int delta)
        {
            foreach (var ns in Namespaces)
            {
                foreach (var cls in ns.Classes)
                {
                    foreach (var meth in cls.Methods)
                    {
                        foreach (var l in meth.Lines)
                        {
                            l.SourceFileID += delta;
                        }
                    }
                }
            }
        }
    }
}
