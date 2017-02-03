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
    class CoverageNodeViewModel : BaseViewModel<CoverageNodeModel>
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


        public CoverageNodeViewModel Parent { get; }

        public ObservableCollection<CoverageNodeViewModel> Children
        {
            get { return _children; }
            private set { Set(ref _children, value); }
        }


        public bool IsVisible
        {
            get { return _isVisible; }
            private set { Set(ref _isVisible, value); }
        }

        public int RowDepth
        {
            get { return _rowDepth; }
            private set { Set(ref _rowDepth, value); }
        }

        public bool CanExpand
        {
            get { return _canExpand; }
            private set { Set(ref _canExpand, value); }
        }

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


        public Brush LineCoverageColor
        {
            get { return _lineCoverageColor; }
            private set { Set(ref _lineCoverageColor, value); }
        }

        public Brush BlockCoverageColor
        {
            get { return _blocksCoverageColor; }
            private set { Set(ref _blocksCoverageColor, value); }
        }


        public RelayCommand ToggleExpandedCmd { get; }


        public CoverageNodeViewModel(CoverageNodeModel model)
            : this(null, model)
        { }

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


        public void ToggleExpanded()
        {
            IsExpanded = !IsExpanded;
        }


        protected override void DetachModel(CoverageNodeModel oldModel)
        {
            oldModel.PropertyChanged -= ModelPropertyChanged;

            Children.CollectionChanged -= ChildrenCollectionChanged;
            Children.Clear();
            Children = null;
        }

        protected override void AttachModel(CoverageNodeModel newModel)
        {
            IsVisible = true;

            if (Parent != null)
            {
                SetupChildren(newModel);
            }

            newModel.PropertyChanged += ModelPropertyChanged;
        }


        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CoverageNodeModel.Name))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private void ChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CanExpand = (Children.Count > 0);
        }


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


        internal void ParentVisibilityChanged()
        {
            IsVisible = DetermineVisibility();

            foreach (var child in Children)
            {
                child.ParentVisibilityChanged();
            }
        }

        private bool DetermineVisibility()
        {
            // check the parent chain for any collapsed nodes.
            bool parentsExpanded = true;

            var parent = Parent;
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
