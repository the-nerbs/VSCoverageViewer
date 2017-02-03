using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Serialization.Export
{
    public class MethodExport
    {
        public string Name { get; set; }
        public int LinesCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }

        public MethodExport() { }

        internal MethodExport(MethodCoverageInfo dat)
        {
            Name = dat.MethodName;
            LinesCovered = dat.LinesCovered;
            LinesPartiallyCovered = dat.LinesPartiallyCovered;
            LinesNotCovered = dat.LinesNotCovered;
            BlocksCovered = dat.BlocksCovered;
            BlocksNotCovered = dat.BlocksNotCovered;
        }
    }
}
