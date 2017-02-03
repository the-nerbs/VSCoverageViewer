using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    public enum Column
    {
        Name,

        [Description("Total Lines")]
        TotalLines,

        [Description("Lines Covered")]
        LinesCovered,

        [Description("Lines Covered %")]
        LinesCoveredPercent,

        [Description("Lines Partially Covered")]
        LinesPartiallyCovered,

        [Description("Lines Partially Covered %")]
        LinesPartiallyCoveredPercent,

        [Description("Lines Not Covered")]
        LinesNotCovered,

        [Description("Lines Not Covered %")]
        LinesNotCoveredPercent,

        [Description("Total Blocks")]
        TotalBlocks,

        [Description("Blocks Covered")]
        BlocksCovered,

        [Description("Blocks Covered %")]
        BlocksCoveredPercent,

        [Description("Blocks Not Covered")]
        BlocksNotCovered,

        [Description("Blocks Not Covered %")]
        BlocksNotCoveredPercent,
    }
}
