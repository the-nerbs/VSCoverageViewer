﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSCoverageViewer.Interfaces;

namespace VSCoverageViewer.Models
{
    /// <summary>
    /// Indicates the type of coverage data an instance of  <see cref="CoverageNodeModel"/>
    /// represents.
    /// </summary>
    internal enum CoverageNodeType
    {
        CoverageFile,
        Module,
        Namespace,
        Type,
        Function,
    }

    /// <summary>
    /// Indicates what code (C#) construct this node represents.
    /// </summary>
    internal enum CodeElementType
    {
        CoverageFile,
        Module,
        Namespace,
        Class,
        Struct,
        Property,
        Function,
    }

    /// <summary>
    /// Model for coverage data nodes.
    /// </summary>
    [DebuggerDisplay("{DebugDisplay,nq}")]
    internal class CoverageNodeModel : ObservableObject
    {
        private string _name;
        private int _linesCovered;
        private int _linesPartiallyCovered;
        private int _linesNotCovered;
        private int _blocksCovered;
        private int _blocksNotCovered;


        /// <summary>
        /// Gets the type of coverage data this represents.
        /// </summary>
        public CoverageNodeType NodeType { get; }

        /// <summary>
        /// Gets code element type this node represents.
        /// </summary>
        public CodeElementType CodeType { get; internal set; }

        /// <summary>
        /// Gets the parent coverage data of this node, or null if this is a root node.
        /// </summary>
        public CoverageNodeModel Parent { get; internal set; }

        /// <summary>
        /// Gets the collection of children of this coverage data.
        /// </summary>
        public ObservableCollection<CoverageNodeModel> Children { get; }


        /// <summary>
        /// Gets or sets the name of this coverage data.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        /// <summary>
        /// Gets the full name of this coverage data, including the module name.
        /// </summary>
        [DependentOn(nameof(Name))]
        public string FullName
        {
            get
            {
                if (Parent != null &&
                    Parent.NodeType != CoverageNodeType.CoverageFile)
                {
                    if (Parent.NodeType != CoverageNodeType.Module)
                    {
                        return Parent.FullName + "." + Name;
                    }

                    // use the IL DASM notation for module-qualified names
                    return "[" + Parent.FullName + "]" + Name;
                }

                return Name;
            }
        }


        /// <summary>
        /// Gets the total number of lines in this coverage node.
        /// </summary>
        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public int TotalLines
        {
            get { return LinesCovered + LinesPartiallyCovered + LinesNotCovered; }
        }

        /// <summary>
        /// Gets or sets the number of lines which are covered.
        /// </summary>
        public int LinesCovered
        {
            get { return _linesCovered; }
            set { Set(ref _linesCovered, value); }
        }

        /// <summary>
        /// Gets or sets the number of lines which are partially covered.
        /// </summary>
        public int LinesPartiallyCovered
        {
            get { return _linesPartiallyCovered; }
            set { Set(ref _linesPartiallyCovered, value); }
        }

        /// <summary>
        /// Gets or sets the number of lines which are not covered.
        /// </summary>
        public int LinesNotCovered
        {
            get { return _linesNotCovered; }
            set { Set(ref _linesNotCovered, value); }
        }


        /// <summary>
        /// Gets the total number of blocks in this coverage node.
        /// </summary>
        [DependentOn(nameof(BlocksCovered))]
        [DependentOn(nameof(BlocksNotCovered))]
        public int TotalBlocks
        {
            get { return BlocksCovered + BlocksNotCovered; }
        }

        /// <summary>
        /// Gets or sets the number of blocks which are covered.
        /// </summary>
        public int BlocksCovered
        {
            get { return _blocksCovered; }
            set { Set(ref _blocksCovered, value); }
        }

        /// <summary>
        /// Gets or sets the number of blocks which are not covered.
        /// </summary>
        public int BlocksNotCovered
        {
            get { return _blocksNotCovered; }
            set { Set(ref _blocksNotCovered, value); }
        }


        /// <summary>
        /// Gets the percentage of lines which are covered.
        /// </summary>
        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public double LinesCoveredRatio
        {
            get { return Ratio(LinesCovered, TotalLines); }
        }


        /// <summary>
        /// Gets the percentage of lines which are partially covered.
        /// </summary>
        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public double LinesPartiallyCoveredRatio
        {
            get { return Ratio(LinesPartiallyCovered, TotalLines); }
        }


        /// <summary>
        /// Gets the percentage of lines which are not covered.
        /// </summary>
        [DependentOn(nameof(LinesCovered))]
        [DependentOn(nameof(LinesPartiallyCovered))]
        [DependentOn(nameof(LinesNotCovered))]
        public double LinesNotCoveredRatio
        {
            get { return Ratio(LinesNotCovered, TotalLines); }
        }


        /// <summary>
        /// Gets the percentage of blocks which are covered.
        /// </summary>
        [DependentOn(nameof(BlocksCovered))]
        [DependentOn(nameof(BlocksNotCovered))]
        public double BlocksCoveredRatio
        {
            get { return Ratio(BlocksCovered, TotalBlocks); }
        }


        /// <summary>
        /// Gets the percentage of blocks which are not covered.
        /// </summary>
        [DependentOn(nameof(BlocksCovered))]
        [DependentOn(nameof(BlocksNotCovered))]
        public double BlocksNotCoveredRatio
        {
            get { return Ratio(BlocksNotCovered, TotalBlocks); }
        }


        /// <summary>
        /// Contains any additional data present in the coverage node.
        /// </summary>
        [Bindable(false)]
        public IDictionary<string, object> AdditionalData { get; }

        /// <summary>
        /// Indicates if .NET metadata for this node has been read or not.
        /// </summary>
        [Bindable(false)]
        public bool HasReadMetadata { get; internal set; }


        /// <summary>
        /// Code for debugger display.
        /// </summary>
        private string DebugDisplay
        {
            get { return $"{Name} : {NodeType}"; }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="CoverageNodeModel"/>.
        /// </summary>
        /// <param name="type">The type of coverage data this represents.</param>
        public CoverageNodeModel(CoverageNodeType type)
        {
            NodeType = type;

            switch (type)
            {
                case CoverageNodeType.CoverageFile:
                    CodeType = CodeElementType.CoverageFile;
                    break;

                case CoverageNodeType.Module:
                    CodeType = CodeElementType.Module;
                    break;

                case CoverageNodeType.Namespace:
                    CodeType = CodeElementType.Namespace;
                    break;

                case CoverageNodeType.Type:
                    CodeType = CodeElementType.Class;
                    break;

                case CoverageNodeType.Function:
                    CodeType = CodeElementType.Function;
                    break;

                default:
                    throw Utility.UnreachableCode("unexpected node type.");
            }

            AdditionalData = new Dictionary<string, object>();

            Children = new ObservableCollection<CoverageNodeModel>();
            Children.CollectionChanged += ChildrenCollectionChanged;
        }


        /// <summary>
        /// Refreshes the coverage counts based on the sum of the all children's counts. If this 
        /// node has no children, this is ignored.
        /// </summary>
        public void RecountCoverage()
        {
            if (Children.Count > 0)
            {
                LinesCovered          = Children.Sum(ch => ch.LinesCovered);
                LinesPartiallyCovered = Children.Sum(ch => ch.LinesPartiallyCovered);
                LinesNotCovered       = Children.Sum(ch => ch.LinesNotCovered);

                BlocksCovered         = Children.Sum(ch => ch.BlocksCovered);
                BlocksNotCovered      = Children.Sum(ch => ch.BlocksNotCovered);
            }
        }

        /// <summary>
        /// Gets the closest ancestor of the given node type.
        /// </summary>
        /// <param name="type">The node type to search for.</param>
        /// <returns>
        /// The coverage node of the given type. If one is not found, <c>null</c> is returned.  If
        /// this node is of the given type, then this node will be returned.
        /// </returns>
        public CoverageNodeModel ClosestAncestor(CoverageNodeType type)
        {
            var node = this;

            while (node != null &&
                   node.NodeType != type)
            {
                node = node.Parent;
            }

            return node;
        }


        /// <summary>
        /// Handles the child collection changing.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">The event details.</param>
        private void ChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (CoverageNodeModel node in e.NewItems)
                    {
                        node.Parent = this;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (CoverageNodeModel node in e.OldItems)
                    {
                        node.Parent = null;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (CoverageNodeModel node in e.OldItems)
                    {
                        node.Parent = null;
                    }

                    foreach (CoverageNodeModel node in e.NewItems)
                    {
                        node.Parent = this;
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Computes a percentage from the value and total amounts.
        /// </summary>
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
