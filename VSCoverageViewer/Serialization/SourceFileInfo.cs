using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VSCoverageViewer.Serialization
{
    public class SourceFileInfo
    {
        [XmlElement("SourceFileID")]
        public int FileID { get; set; }

        [XmlElement("SourceFileName")]
        public string FileName { get; set; }


        internal SourceFileInfo Clone()
        {
            return (SourceFileInfo)MemberwiseClone();
        }
    }
}
