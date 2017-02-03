using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VSCoverageViewer.Models;

namespace VSCoverageViewer.Serialization.Export
{
    [XmlRoot("CovProj")]
    public class CoverageExport
    {
        public string Name { get; set; }
        public int LinesCovered { get; set; }
        public int LinesPartiallyCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }
        public ModuleExport[] Modules { get; set; }

        public CoverageExport() { }

        internal CoverageExport(string name, CoverageDSPriv exportData)
        {
            Name = name;

            var modules = new List<ModuleExport>();

            foreach (ModuleCoverageInfo mod in exportData.Modules)
            {
                modules.Add(new ModuleExport(mod));

                LinesCovered += mod.LinesCovered;
                LinesPartiallyCovered += mod.LinesPartiallyCovered;
                LinesNotCovered += mod.LinesNotCovered;
                BlocksCovered += mod.BlocksCovered;
                BlocksNotCovered += mod.BlocksNotCovered;
            }

            Modules = modules.ToArray();
        }
    }
}
