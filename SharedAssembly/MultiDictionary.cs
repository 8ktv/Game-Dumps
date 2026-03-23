using System.Collections.Generic;
using System.Linq;

public class MultiDictionary<TKey, TValue, TCollection> where TCollection : ICollection<TValue>, new()
{
	private readonly Dictionary<TKey, TCollection> internalDictionary = new Dictionary<TKey, TCollection>();

	public IEnumerable<TKey> Keys => internalDictionary.Keys;

	public IEnumerable<TCollection> Collections => internalDictionary.Values;

	public int KeyCount => internalDictionary.Keys.Count;

	public int ValueCount
	{
		get
		{
			int num = 0;
			foreach (TCollection value in internalDictionary.Values)
			{
				num += value.Count();
			}
			return num;
		}
	}

	public void Clear()
	{
		internalDictionary.Clear();
	}

	public bool ContainsKey(TKey key)
	{
		return internalDictionary.ContainsKey(key);
	}

	public void Add(TKey key, TValue value)
	{
		if (!internalDictionary.TryGetValue(key, out var value2))
		{
			value2 = new TCollection();
			internalDictionary.Add(key, value2);
		}
		value2.Add(value);
	}

	public void Remove(TKey key, TValue value)
	{
		if (internalDictionary.TryGetValue(key, out var value2))
		{
			value2.Remove(value);
			if (value2.Count == 0)
			{
				internalDictionary.Remove(key);
			}
		}
	}

	public void RemoveAll(TKey key)
	{
		if (internalDictionary.TryGetValue(key, out var value) && value.Count == 0)
		{
			internalDictionary.Remove(key);
		}
	}

	public bool TryGetValues(TKey key, out TCollection values)
	{
		if (internalDictionary.TryGetValue(key, out var value))
		{
			values = value;
			return true;
		}
		values = default(TCollection);
		return false;
	}

	public bool TryGetFirstValue(TKey key, out TValue value)
	{
		if (internalDictionary.TryGetValue(key, out var value2))
		{
			value = value2.First();
			return true;
		}
		value = default(TValue);
		return false;
	}

	public IEnumerable<TValue> GetAllValues()
	{
		List<TValue> list = new List<TValue>();
		foreach (TCollection value in internalDictionary.Values)
		{
			list.AddRange(value);
		}
		return list;
	}
}
