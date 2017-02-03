using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    public enum ColumnPreset
    {
        All,

        [Description("All (Totals)")]
        AllTotals,

        [Description("All (%)")]
        AllRatios,


        Lines,

        [Description("Lines (Totals)")]
        LineTotals,

        [Description("Lines (%)")]
        LineRatios,


        Blocks,

        [Description("Blocks (Totals)")]
        BlockTotals,

        [Description("Block (%)")]
        BlockRatios,


        [Bindable(false)]
        Custom,
    }
}
