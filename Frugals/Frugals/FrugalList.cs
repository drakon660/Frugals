using System.Collections;

namespace Frugals;

public class FrugalList<T> : IEnumerable<T>
{
    private T _singleItem;
    private T[] _multiItems;
    private bool _hasSingleItem;

    public int Count { get; private set; }
    
    public T SingleItem => _hasSingleItem ? _singleItem : default;

    public FrugalList(int count)
    {
        _multiItems = new T[count];
    }
    
    public void Add(T item)
    {
        if (Count == 0)
        {
            _singleItem = item; // Store as single value
            _hasSingleItem = true;
        }
        else if (Count == 1)
        {
            _multiItems = [_singleItem, item ]; // Convert to list
            _hasSingleItem = false;
        }
        else
        {
            _multiItems[Count] =item; // Use List<T> for 2+ items
        }

        Count++;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (Count == 1) yield return _singleItem;
        else if (Count > 1)
            foreach (var item in _multiItems)
                yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class FrugalTaskList<TResult> : FrugalList<Task<TResult>>
{
    public FrugalTaskList(int count) : base(count)
    {
        
    }

    public async Task<TResult[]> WaitAllAndGetResultsAsync()
    {
        switch (Count)
        {
            case 0:
                return [];
            case 1:
                return [await SingleItem];
            default:
                // Convert to array and use Task.WhenAll
                return await Task.WhenAll(this).ConfigureAwait(false);
        }
    }
}