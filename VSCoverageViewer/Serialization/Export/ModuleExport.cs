using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Serialization.Export
{
    public class ModuleExport
    {
        public string Name { get; set; }
        public int LinesCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }
        public NamespaceExport[] Namespaces { get; set; }

        public ModuleExport() { }

        internal ModuleExport(ModuleCoverageInfo dat)
        {
            Name = dat.ModuleName;

            Namespaces = dat.Namespaces.Select(c => new NamespaceExport(c)).ToArray();
            LinesCovered = dat.LinesCovered;
            LinesPartiallyCovered = dat.LinesPartiallyCovered;
            LinesNotCovered = dat.LinesNotCovered;
            BlocksCovered = dat.BlocksCovered;
            BlocksNotCovered = dat.BlocksNotCovered;
        }
    }
}
