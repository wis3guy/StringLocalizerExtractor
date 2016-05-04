#region # using statements #

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace StringLocalizerExtractor
{

    /// <summary>
    /// Represents a read-only collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ReadOnlyCollection<T> : IEnumerable<T>
    {
        #region # Variables #

        internal static readonly ReadOnlyCollection<T> Empty =
            new ReadOnlyCollection<T>(new T[0]);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly
            IEnumerable<T> mItems;

        #endregion

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ReadOnlyCollection{T}"/> class.
        /// </summary>
        /// <param name="items"></param>
        internal ReadOnlyCollection(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var itemsArray = items as T[] ?? items.ToArray();
            mItems = itemsArray;
            Count = itemsArray.Length;
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the number of items this collection contains.
        /// </summary>
        public int Count { get; }

        #endregion

        #endregion

        #region # IEnumerable #

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region # IEnumerable<T> #

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}" /> that can be used to iterate through
        /// the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion
    }
}