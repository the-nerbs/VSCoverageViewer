using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    class ViewModelCollection<TModel, TViewModel> : ObservableCollection<TViewModel>
    {
        private readonly ObservableCollection<TModel> _boundCollection;
        private readonly Func<TModel, TViewModel> _factory;


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
