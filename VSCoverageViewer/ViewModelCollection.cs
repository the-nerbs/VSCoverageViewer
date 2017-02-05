using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSCoverageViewer.ViewModels;

namespace VSCoverageViewer
{
    /// <summary>
    /// A subclass of <see cref="ObservableCollection{T}"/> which keeps this collection in sync 
    /// with another collection.  This is intended to be used to keep a collection of view models
    /// in sync with a collection of models.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    internal class ViewModelCollection<TModel, TViewModel> : ObservableCollection<TViewModel>
        where TModel : ObservableObject
        where TViewModel : BaseViewModel<TModel>
    {
        private readonly ObservableCollection<TModel> _boundCollection;
        private readonly Func<TModel, TViewModel> _factory;
        private bool _ignoreUpdates = false;


        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelCollection{TModel, TViewModel}"/>.
        /// </summary>
        /// <param name="boundCollection">The collection to bind this to.</param>
        /// <param name="vmFactory">A function which gets a view model for a model.</param>
        public ViewModelCollection(ObservableCollection<TModel> boundCollection, Func<TModel, TViewModel> vmFactory)
        {
            if (boundCollection == null)
                throw new ArgumentNullException(nameof(boundCollection));

            _boundCollection = boundCollection;
            _factory = vmFactory;

            _boundCollection.CollectionChanged += BoundCollectionChanged;

            using (new DisableRecursiveUpdates(this))
            {
                foreach (var item in boundCollection)
                {
                    Add(_factory(item));
                }
            }
        }


        /// <summary>
        /// Handles this collection changing and forwards changes to the bound collection.
        /// </summary>
        /// <param name="e">The change details.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_ignoreUpdates)
                return;

            base.OnCollectionChanged(e);

            // disable updates caused what we do to the bound collection here.
            using (new DisableRecursiveUpdates(this))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            var viewModel = (TViewModel)e.NewItems[i];
                            _boundCollection.Insert(i + e.NewStartingIndex, viewModel.Model);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            _boundCollection.RemoveAt(e.OldStartingIndex);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            var viewModel = (TViewModel)e.NewItems[i];
                            _boundCollection[i + e.NewStartingIndex] = viewModel.Model;
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        if (e.NewItems.Count > 1)
                            throw new NotSupportedException("Range moves are not supported.");

                        var model = _boundCollection[e.OldStartingIndex];
                        _boundCollection.RemoveAt(e.OldStartingIndex);
                        _boundCollection.Insert(e.NewStartingIndex, model);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _boundCollection.Clear();
                        foreach (var vm in this)
                        {
                            _boundCollection.Add(vm.Model);
                        }
                        break;

                    default:
                        throw Utility.UnreachableCode($"Unrecognized collection change action: {e.Action}.");
                }
            }
        }


        /// <summary>
        /// Handles the model collection changing.
        /// </summary>
        /// <param name="sender">The model collection.</param>
        /// <param name="e">The change details.</param>
        private void BoundCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_ignoreUpdates)
                return;

            using (new DisableRecursiveUpdates(this))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            Insert(i + e.NewStartingIndex, _factory((TModel)e.NewItems[i]));
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            RemoveAt(e.OldStartingIndex);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            this[i + e.NewStartingIndex] = _factory((TModel)e.NewItems[i]);
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        if (e.NewItems.Count > 1)
                            throw new NotSupportedException("Range moves are not supported.");

                        var vm = this[e.OldStartingIndex];
                        RemoveAt(e.OldStartingIndex);
                        Insert(e.NewStartingIndex, vm);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        Clear();
                        foreach (var model in _boundCollection)
                        {
                            Add(_factory(model));
                        }
                        break;

                    default:
                        throw Utility.UnreachableCode($"Unrecognized collection change action: {e.Action}.");
                }
            }
        }

        /// <summary>
        /// Helper class for disabling recursive changes.
        /// </summary>
        private class DisableRecursiveUpdates : IDisposable
        {
            private readonly ViewModelCollection<TModel, TViewModel> _collection;
            private readonly bool _previousIgnoreFlag;


            /// <summary>
            /// Initializes a new instance of <see cref="DisableRecursiveUpdates"/> and disables
            /// updates in the collection.
            /// </summary>
            /// <param name="collection">The collection to disable updates for.</param>
            public DisableRecursiveUpdates(ViewModelCollection<TModel, TViewModel> collection)
            {
                _collection = collection;

                _previousIgnoreFlag = _collection._ignoreUpdates;
                _collection._ignoreUpdates = true;
            }

            /// <summary>
            /// Restores updates for the collection this was constructed with.
            /// </summary>
            public void Dispose()
            {
                _collection._ignoreUpdates = _previousIgnoreFlag;
            }
        }
    }
}
