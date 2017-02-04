using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// Contains definitions for column visibility presets.
    /// </summary>
    internal static class ColumnPresetLists
    {
        #region Presets

        private static readonly Dictionary<ColumnPreset, IList<Column>> Presets =
            new Dictionary<ColumnPreset, IList<Column>>()
            {
                [ColumnPreset.All] =
                    Enum.GetValues(typeof(Column)).Cast<Column>().ToArray(),

                [ColumnPreset.AllTotals] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalLines, Column.LinesCovered, Column.LinesPartiallyCovered, Column.LinesNotCovered,
                            Column.TotalBlocks, Column.BlocksCovered, Column.BlocksNotCovered,
                    },

                [ColumnPreset.AllRatios] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalLines, Column.LinesCoveredPercent, Column.LinesPartiallyCoveredPercent, Column.LinesNotCoveredPercent,
                            Column.TotalBlocks, Column.BlocksCoveredPercent, Column.BlocksNotCoveredPercent,
                    },

                [ColumnPreset.Lines] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalLines, Column.LinesCovered, Column.LinesPartiallyCovered, Column.LinesNotCovered,
                            Column.LinesCoveredPercent, Column.LinesPartiallyCoveredPercent, Column.LinesNotCoveredPercent,
                    },

                [ColumnPreset.LineTotals] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalLines, Column.LinesCovered, Column.LinesPartiallyCovered, Column.LinesNotCovered,
                    },

                [ColumnPreset.LineRatios] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalLines, Column.LinesCoveredPercent, Column.LinesPartiallyCoveredPercent, Column.LinesNotCoveredPercent,
                    },

                [ColumnPreset.Blocks] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalBlocks, Column.BlocksCovered, Column.BlocksNotCovered,
                            Column.BlocksCoveredPercent, Column.BlocksNotCoveredPercent,
                    },

                [ColumnPreset.BlockTotals] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalBlocks, Column.BlocksCovered, Column.BlocksNotCovered,
                    },

                [ColumnPreset.BlockRatios] =
                    new[]
                    {
                            Column.Name,
                            Column.TotalBlocks, Column.BlocksCoveredPercent, Column.BlocksNotCoveredPercent,
                    },
            };

        #endregion


        /// <summary>
        /// Gets the collection of columns which are visible for the given preset.
        /// </summary>
        /// <param name="preset">The preset.</param>
        /// <returns>The collection of visible columns.</returns>
        internal static IEnumerable<Column> GetPreset(ColumnPreset preset)
        {
            Debug.Assert(preset != ColumnPreset.Custom);

            return Presets[preset];
        }
    }
}
