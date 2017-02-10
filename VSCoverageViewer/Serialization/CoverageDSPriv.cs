using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VSCoverageViewer.Serialization
{
    public class CoverageDSPriv
    {
        [XmlElement("Module")]
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "Serialization type.")]
        public ModuleCoverageInfo[] Modules { get; set; }
        
        [XmlElement("SourceFileNames")]
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "Serialization type.")]
        public SourceFileInfo[] SourceFileNames { get; set; }



        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces
        {
            get { return NamespacesInternal; }
        }

        internal static XmlSerializerNamespaces NamespacesInternal
        {
            get
            {
                var nsTable = new XmlSerializerNamespaces();
                nsTable.Add("", "urn:Coverage");
                return nsTable;
            }
        }
    }
}
