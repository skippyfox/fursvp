// <copyright file="IEnumerableExtensions.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains extension methods for IEnumerable types.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Correlates the elements of two sequences based on matching keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An IEnumerable that has elements of type TResult that are obtained by performing a full outer join on two sequences.</returns>
        public static IEnumerable<TResult> FullJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            if (outer == null)
            {
                throw new ArgumentNullException(nameof(outer));
            }

            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            if (outerKeySelector == null)
            {
                throw new ArgumentNullException(nameof(outerKeySelector));
            }

            if (innerKeySelector == null)
            {
                throw new ArgumentNullException(nameof(innerKeySelector));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var results = new List<TResult>();

            var outerList = outer.Select(o => new { Value = o, Key = outerKeySelector(o) }).ToList();
            var innerList = inner.Select(i => new Match<TKey, TInner> { Value = i, Key = innerKeySelector(i) }).ToList();

            foreach (var o in outerList)
            {
                var innerMatches = innerList.Where(i => Equals(o.Key, i.Key)).ToList();

                if (innerMatches.Any())
                {
                    foreach (var innerMatch in innerMatches)
                    {
                        innerMatch.Matched = true;
                        yield return resultSelector(o.Value, innerMatch.Value);
                    }
                }
                else
                {
                    yield return resultSelector(o.Value, default);
                }
            }

            foreach (var i in innerList.Where(i => !i.Matched))
            {
                yield return resultSelector(default, i.Value);
            }
        }

        private class Match<TKey, TObject>
        {
            public TKey Key { get; set; }

            public TObject Value { get; set; }

            public bool Matched { get; set; }
        }
    }
}
