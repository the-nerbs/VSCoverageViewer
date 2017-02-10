using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Serialization.Export
{
    public class NamespaceExport
    {
        public string Name { get; set; }
        public int LinesCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "Serialization type.")]
        public ClassExport[] Classes { get; set; }


        public NamespaceExport() { }

        internal NamespaceExport(NamespaceCoverageInfo dat)
        {
            Name = dat.NamespaceName;

            Classes = dat.Classes.Select(c => new ClassExport(c)).ToArray();
            LinesCovered = dat.LinesCovered;
            LinesPartiallyCovered = dat.LinesPartiallyCovered;
            LinesNotCovered = dat.LinesNotCovered;
            BlocksCovered = dat.BlocksCovered;
            BlocksNotCovered = dat.BlocksNotCovered;
        }
    }
}
