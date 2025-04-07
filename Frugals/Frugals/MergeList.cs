using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Frugals;

/// <summary>
/// A specialized collection that merges multiple lists of the same type based on custom predicates.
/// </summary>
/// <typeparam name="T">The type of elements in all lists</typeparam>
public class MergeList<T> : IReadOnlyList<T>
{
    private readonly List<IReadOnlyList<T>> _sourceLists;
    private readonly Func<T, T, bool> _shouldMerge;
    private readonly Func<T, T, T> _merger;
    private List<T> _mergedCache;

    /// <summary>
    /// Creates a new MergeList that combines multiple source collections.
    /// </summary>
    /// <param name="shouldMerge">Predicate that determines when two items should be merged</param>
    /// <param name="merger">Function that defines how to merge two matching items</param>
    /// <param name="sourceLists">The collections to merge</param>
    public MergeList(Func<T, T, bool> shouldMerge, Func<T, T, T> merger, params IReadOnlyList<T>[] sourceLists)
    {
        _shouldMerge = shouldMerge ?? throw new ArgumentNullException(nameof(shouldMerge));
        _merger = merger ?? throw new ArgumentNullException(nameof(merger));
        _sourceLists = sourceLists?.ToList() ?? new List<IReadOnlyList<T>>();
    }

    /// <summary>
    /// Creates a new MergeList using a key selector for merging.
    /// Items are merged when they have the same key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used for matching</typeparam>
    /// <param name="keySelector">Function to extract the key used for matching</param>
    /// <param name="merger">Function that defines how to merge two matching items</param>
    /// <param name="sourceLists">The collections to merge</param>
    public static MergeList<T> CreateByKey<TKey>(
        Func<T, TKey> keySelector,
        Func<T, T, T> merger,
        params IReadOnlyList<T>[] sourceLists)
        where TKey : IEquatable<TKey>
    {
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        return new MergeList<T>(
            (a, b) => EqualityComparer<TKey>.Default.Equals(keySelector(a), keySelector(b)),
            merger,
            sourceLists);
    }

    /// <summary>
    /// Creates a new MergeList that takes the first item when matches are found.
    /// </summary>
    /// <param name="shouldMerge">Predicate that determines when two items should be merged</param>
    /// <param name="sourceLists">The collections to merge</param>
    public static MergeList<T> CreateTakeFirst(
        Func<T, T, bool> shouldMerge,
        params IReadOnlyList<T>[] sourceLists)
    {
        return new MergeList<T>(shouldMerge, (first, _) => first, sourceLists);
    }

    /// <summary>
    /// Creates a new MergeList that takes the last item when matches are found.
    /// </summary>
    /// <param name="shouldMerge">Predicate that determines when two items should be merged</param>
    /// <param name="sourceLists">The collections to merge</param>
    public static MergeList<T> CreateTakeLast(
        Func<T, T, bool> shouldMerge,
        params IReadOnlyList<T>[] sourceLists)
    {
        return new MergeList<T>(shouldMerge, (_, last) => last, sourceLists);
    }

    /// <summary>
    /// Total count of items in the merged list.
    /// </summary>
    public int Count => GetMergedList().Count;

    /// <summary>
    /// Gets the item at the specified index in the merged list.
    /// </summary>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return GetMergedList()[index];
        }
    }

    /// <summary>
    /// Gets the merged list, creating it if necessary.
    /// </summary>
    private List<T> GetMergedList()
    {
        if (_mergedCache != null)
            return _mergedCache;

        // Fast path for empty or single list
        if (_sourceLists.Count == 0)
            return _mergedCache = new List<T>();

        if (_sourceLists.Count == 1)
            return _mergedCache = new List<T>(_sourceLists[0]);

        // Create a flat list of all items
        var allItems = new List<T>();
        foreach (var list in _sourceLists)
        {
            allItems.AddRange(list);
        }

        // Now perform the merge based on the predicate
        _mergedCache = MergeItems(allItems);
        return _mergedCache;
    }

    /// <summary>
    /// Merges items in the list based on the shouldMerge predicate.
    /// </summary>
    private List<T> MergeItems(List<T> items)
    {
        if (items.Count <= 1)
            return new List<T>(items);

        var result = new List<T>(items.Count);
        var processed = new bool[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            if (processed[i])
                continue;

            T current = items[i];
            processed[i] = true;

            // Find all matching items and merge them
            for (int j = i + 1; j < items.Count; j++)
            {
                if (processed[j])
                    continue;

                if (_shouldMerge(current, items[j]))
                {
                    current = _merger(current, items[j]);
                    processed[j] = true;
                }
            }

            result.Add(current);
        }

        return result;
    }

    /// <summary>
    /// Returns an enumerator for the merged list.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        return GetMergedList().GetEnumerator();
    }

    /// <summary>
    /// Returns a non-generic enumerator for the merged list.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Forces regeneration of the merged list on next access.
    /// </summary>
    public void InvalidateCache()
    {
        _mergedCache = null;
    }
}