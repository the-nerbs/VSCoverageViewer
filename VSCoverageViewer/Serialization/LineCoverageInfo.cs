using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VSCoverageViewer.Serialization
{
    public class LineCoverageInfo
    {
        public int LnStart { get; set; }
        public int ColStart { get; set; }

        public int LnEnd { get; set; }
        public int ColEnd { get; set; }

        public int Coverage { get; set; }
        public int SourceFileID { get; set; }
        public int LineID { get; set; }


        internal LineCoverageInfo Clone()
        {
            return (LineCoverageInfo)MemberwiseClone();
        }
    }
}
