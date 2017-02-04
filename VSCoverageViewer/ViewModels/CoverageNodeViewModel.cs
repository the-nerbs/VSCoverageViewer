using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight.CommandWpf;
using VSCoverageViewer.Messaging;
using VSCoverageViewer.Models;

namespace VSCoverageViewer.ViewModels
{
    /// <summary>
    /// View model for coverage nodes.
    /// </summary>
    internal class CoverageNodeViewModel : BaseViewModel<CoverageNodeModel>
    {
        private static readonly SolidColorBrush CoveredColor = Brushes.LightGreen;
        private static readonly SolidColorBrush PartiallyCoveredColor = Brushes.LemonChiffon;
        private static readonly SolidColorBrush NotCoveredColor = Brushes.Pink;


        private ObservableCollection<CoverageNodeViewModel> _children;
        private bool _isVisible;
        private int _rowDepth;
        private bool _canExpand;
        private bool _isExpanded;
        private Brush _lineCoverageColor;
        private Brush _blocksCoverageColor;


        /// <summary>
        /// Gets the parent coverage node's view model.
        /// </summary>
        public CoverageNodeViewModel Parent { get; }

        /// <summary>
        /// Gets the collection of child coverage node view models.
        /// </summary>
        public ObservableCollection<CoverageNodeViewModel> Children
        {
            get { return _children; }
            private set { SetIfChanged(ref _children, value); }
        }

        /// <summary>
        /// Gets the visibility of this node.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            private set { SetIfChanged(ref _isVisible, value); }
        }

        /// <summary>
        /// Gets the depth of this row.
        /// </summary>
        public int RowDepth
        {
            get { return _rowDepth; }
            private set { SetIfChanged(ref _rowDepth, value); }
        }

        /// <summary>
        /// Gets a value indicating if this node can be expanded.
        /// </summary>
        public bool CanExpand
        {
            get { return _canExpand; }
            private set { SetIfChanged(ref _canExpand, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if this node is currently expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (CanExpand &&
                    _isExpanded != value)
                {
                    Set(ref _isExpanded, value);

                    foreach (var child in Children)
                    {
                        child.ParentVisibilityChanged();
                    }
                }
            }
        }


        /// <summary>
        /// Gets the display name of this node.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Model.NodeType == CoverageNodeType.CoverageFile)
                {
                    return System.IO.Path.GetFileNameWithoutExtension(Model.Name);
                }

                return Model.Name;
            }
        }


        /// <summary>
        /// Gets the color to highlight line coverage with.
        /// </summary>
        public Brush LineCoverageColor
        {
            get { return _lineCoverageColor; }
            private set { Set(ref _lineCoverageColor, value); }
        }

        /// <summary>
        /// Gets the color to highlight block coverage with.
        /// </summary>
        public Brush BlockCoverageColor
        {
            get { return _blocksCoverageColor; }
            private set { Set(ref _blocksCoverageColor, value); }
        }


        /// <summary>
        /// Gets a command which toggles if this node is expanded.
        /// </summary>
        public RelayCommand ToggleExpandedCmd { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="CoverageNodeViewModel"/>.
        /// </summary>
        /// <param name="model">The model to initialize with.</param>
        public CoverageNodeViewModel(CoverageNodeModel model)
            : this(null, model)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="CoverageNodeViewModel"/>.
        /// </summary>
        /// <param name="model">The model to initialize with.</param>
        /// <param name="parent">The parent coverage row's view model.</param>
        public CoverageNodeViewModel(CoverageNodeViewModel parent, CoverageNodeModel model)
            : base(model)
        {
            Parent = parent;
            IsExpanded = false;
            IsVisible = DetermineVisibility();

            ToggleExpandedCmd = new RelayCommand(ToggleExpanded);

            SetupChildren(model);

            int depth = 0;
            var node = parent;
            while (node != null)
            {
                node = node.Parent;
                depth++;
            }

            RowDepth = depth;


            Messenger.Register<ThresholdChangedMessage>(this, HandleThresholdChanged);
        }


        /// <summary>
        /// Toggles the expanded state of this node.
        /// </summary>
        public void ToggleExpanded()
        {
            IsExpanded = !IsExpanded;
        }


        /// <summary>
        /// Attaches a new model to this view model.
        /// </summary>
        /// <param name="newModel">The model to attach.</param>
        protected override void AttachModel(CoverageNodeModel newModel)
        {
            IsVisible = true;

            if (Parent != null)
            {
                SetupChildren(newModel);
            }

            newModel.PropertyChanged += ModelPropertyChanged;
        }

        /// <summary>
        /// Detaches a model from this view model.
        /// </summary>
        /// <param name="oldModel">The model to detach.</param>
        protected override void DetachModel(CoverageNodeModel oldModel)
        {
            oldModel.PropertyChanged -= ModelPropertyChanged;

            Children.CollectionChanged -= ChildrenCollectionChanged;
            Children.Clear();
            Children = null;
        }


        /// <summary>
        /// Handles a property of the model changing.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">The property change details.</param>
        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // forward model name changes to view model display name.
            if (e.PropertyName == nameof(CoverageNodeModel.Name))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        /// <summary>
        /// Handles the children collection changing.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void ChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Keep CanExpand in sync with the child collection
            CanExpand = (Children.Count > 0);
        }


        /// <summary>
        /// Sets up the child collection.
        /// </summary>
        /// <param name="model">The model to sync with.</param>
        private void SetupChildren(CoverageNodeModel model)
        {
            Children = new ViewModelCollection<CoverageNodeModel, CoverageNodeViewModel>(
                model.Children,
                (m) =>
                {
                    Debug.Assert(m != null);
                    //Debug.Assert(m.Parent == model);

                    return new CoverageNodeViewModel(this, m);
                }
            );

            Children.CollectionChanged += ChildrenCollectionChanged;
            CanExpand = (Children.Count > 0);
        }

        /// <summary>
        /// Handles <see cref="ThresholdChangedMessage"/> notifications.
        /// </summary>
        /// <param name="msg">The message data.</param>
        private void HandleThresholdChanged(ThresholdChangedMessage msg)
        {
            if (Model.LinesCoveredRatio >= msg.NewThresholdRatio)
            {
                LineCoverageColor = CoveredColor;
            }
            else if (Model.LinesPartiallyCoveredRatio >= msg.NewThresholdRatio)
            {
                LineCoverageColor = PartiallyCoveredColor;
            }
            else
            {
                LineCoverageColor = NotCoveredColor;
            }


            if (Model.BlocksCoveredRatio >= msg.NewThresholdRatio)
            {
                BlockCoverageColor = CoveredColor;
            }
            else
            {
                BlockCoverageColor = NotCoveredColor;
            }
        }


        /// <summary>
        /// Handles the parent visibility changing.
        /// </summary>
        internal void ParentVisibilityChanged()
        {
            IsVisible = DetermineVisibility();

            foreach (var child in Children)
            {
                child.ParentVisibilityChanged();
            }
        }

        /// <summary>
        /// Sets the expanded state but does not notify children. Used when rows are expanded or
        /// collapsed en mass (such as expand/collapse all) to avoid evaluating the visibility
        /// of child nodes once for each of its ancestors.
        /// </summary>
        /// <param name="isExpanded">The expanded state of this node.</param>
        internal void SetExpandedWithoutNotifyingChildren(bool isExpanded)
        {
            SetIfChanged(ref _isExpanded, isExpanded, nameof(IsExpanded));
            IsVisible = DetermineVisibility();
        }


        /// <summary>
        /// Determines the visibility of this node.
        /// </summary>
        /// <returns>True if this node is visible, or false if not.</returns>
        private bool DetermineVisibility()
        {
            // check the parent chain for any collapsed nodes.
            bool parentsExpanded = true;

            CoverageNodeViewModel parent = Parent;

            while (parentsExpanded &&
                   parent != null)
            {
                parentsExpanded = parent.IsExpanded;
                parent = parent.Parent;
            }
            
            // a parent somewhere up the chain is collapsed; this is not visible.
            if (!parentsExpanded)
            {
                return false;
            }
            
            return true;
        }
    }
}
