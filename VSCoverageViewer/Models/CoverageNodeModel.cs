using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSCoverageViewer.Interfaces;

namespace VSCoverageViewer.Models
{
    enum CoverageNodeType
    {
        CoverageFile,
        Module,
        Namespace,
        Class,
        Function,
    }

    /// <summary>
    /// Model for 
    /// </summary>
    class CoverageNodeModel : ObservableObject
    {
        private string _name;
        private int _linesCovered;
        private int _linesPartiallyCovered;
        private int _linesNotCovered;
        private int _blocksCovered;
        private int _blocksNotCovered;


        public CoverageNodeType NodeType { get; }

        public CoverageNodeModel Parent { get; internal set; }

        public ObservableCollection<CoverageNodeModel> Children { get; }


        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        [DependentOn(nameof(Name))]
        public string FullName
        {
            get
            {
                if (Parent != null &&
                    Parent.NodeType != CoverageNodeType.CoverageFile)
                {
                    return Parent.FullName + "." + Name;
                }

                return Name;
            }
        }



        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public int TotalLines
        {
            get { return LinesCovered + LinesPartiallyCovered + LinesNotCovered; }
        }

        public int LinesCovered
        {
            get { return _linesCovered; }
            set { Set(ref _linesCovered, value); }
        }

        public int LinesPartiallyCovered
        {
            get { return _linesPartiallyCovered; }
            set { Set(ref _linesPartiallyCovered, value); }
        }

        public int LinesNotCovered
        {
            get { return _linesNotCovered; }
            set { Set(ref _linesNotCovered, value); }
        }


        [DependentOn(nameof(BlocksCovered))]
        [DependentOn(nameof(BlocksNotCovered))]
        public int TotalBlocks
        {
            get { return BlocksCovered + BlocksNotCovered; }
        }

        public int BlocksCovered
        {
            get { return _blocksCovered; }
            set { Set(ref _blocksCovered, value); }
        }

        public int BlocksNotCovered
        {
            get { return _blocksNotCovered; }
            set { Set(ref _blocksNotCovered, value); }
        }


        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public double LinesCoveredRatio
        {
            get { return Ratio(LinesCovered, TotalLines); }
        }

        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public double LinesPartiallyCoveredRatio
        {
            get { return Ratio(LinesPartiallyCovered, TotalLines); }
        }

        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public double LinesNotCoveredRatio
        {
            get { return Ratio(LinesNotCovered, TotalLines); }
        }


        [DependentOn(nameof(BlocksCovered))]
        [DependentOn(nameof(BlocksNotCovered))]
        public double BlocksCoveredRatio
        {
            get { return Ratio(BlocksCovered, TotalBlocks); }
        }

        [DependentOn(nameof(BlocksCovered))]
        [DependentOn(nameof(BlocksNotCovered))]
        public double BlocksNotCoveredRatio
        {
            get { return Ratio(BlocksNotCovered, TotalBlocks); }
        }

        /// <summary>
        /// Contains any additional data present in the coverage node.
        /// </summary>
        public IDictionary<string, object> AdditionalData { get; }


        public CoverageNodeModel(CoverageNodeType type)
        {
            NodeType = type;

            Children = new ObservableCollection<CoverageNodeModel>();
            AdditionalData = new Dictionary<string, object>();
        }



        private static double Ratio(double value, double total)
        {
            if (total == 0)
            {
                return 0.0;
            }

            return value / total;
        }
    }
}
