using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    {
        private readonly ObservableCollection<TModel> _boundCollection;
        private readonly Func<TModel, TViewModel> _factory;


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

            foreach (var item in boundCollection)
            {
                Add(_factory(item));
            }
        }


        private void BoundCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
}
