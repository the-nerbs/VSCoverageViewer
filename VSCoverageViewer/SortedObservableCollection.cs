using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// Extension of <see cref="ObservableCollection{T}"/> which inserts items in sorted order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SortedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly IComparer<T> _comparer;


        /// <summary>
        /// Initializes a new instance of <see cref="SortedDictionary{TKey, TValue}"/>, sorted
        /// using the default comparer for <typeparamref name="T"/>.
        /// </summary>
        public SortedObservableCollection()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SortedDictionary{TKey, TValue}"/>, sorted
        /// using the given comparer for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="comparer">The comparer used to sort.</param>
        public SortedObservableCollection(IComparer<T> comparer)
        {
            if (comparer == null)
            {
                try
                {
                    comparer = Comparer<T>.Default;
                }
                catch
                {
                    // unfortunately, ArgumentNullException does not have a constructor taking 
                    // all three (paramName, message, innerException), so there's no point to
                    // capturing the exception here...
                    comparer = null;
                }

                Contract.Requires<ArgumentNullException>(
                    comparer != null,
                    nameof(comparer),
                    $"Cannot determine default comparison for type {typeof(T).FullName}");
            }

            _comparer = comparer;
        }


        /// <summary>
        /// Inserts an item in sorted order.
        /// </summary>
        /// <param name="index">Ignored.</param>
        /// <param name="item">The item to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            int idx = 0;

            while (idx < Count &&
                   _comparer.Compare(Items[idx], item) <= 0)
            {
                idx++;
            }

            base.InsertItem(idx, item);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException();
        }
    }
}
