using System;
using System.Collections.Generic;
using System.Text;

namespace Mirror.BouncyCastle.Utilities.Collections;

public abstract class CollectionUtilities
{
	public static void CollectMatches<T>(ICollection<T> matches, ISelector<T> selector, IEnumerable<IStore<T>> stores)
	{
		if (matches == null)
		{
			throw new ArgumentNullException("matches");
		}
		if (stores == null)
		{
			return;
		}
		foreach (IStore<T> store in stores)
		{
			if (store == null)
			{
				continue;
			}
			foreach (T item in store.EnumerateMatches(selector))
			{
				matches.Add(item);
			}
		}
	}

	public static IStore<T> CreateStore<T>(IEnumerable<T> contents)
	{
		return new StoreImpl<T>(contents);
	}

	public static T GetFirstOrNull<T>(IEnumerable<T> e) where T : class
	{
		if (e != null)
		{
			using IEnumerator<T> enumerator = e.GetEnumerator();
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public static T GetValueOrKey<T>(IDictionary<T, T> d, T k)
	{
		if (!d.TryGetValue(k, out var value))
		{
			return k;
		}
		return value;
	}

	public static V GetValueOrNull<K, V>(IDictionary<K, V> d, K k) where V : class
	{
		if (!d.TryGetValue(k, out var value))
		{
			return null;
		}
		return value;
	}

	public static IEnumerable<T> Proxy<T>(IEnumerable<T> e)
	{
		return new EnumerableProxy<T>(e);
	}

	public static ICollection<T> ReadOnly<T>(ICollection<T> c)
	{
		return new ReadOnlyCollectionProxy<T>(c);
	}

	public static IDictionary<K, V> ReadOnly<K, V>(IDictionary<K, V> d)
	{
		return new ReadOnlyDictionaryProxy<K, V>(d);
	}

	public static IList<T> ReadOnly<T>(IList<T> l)
	{
		return new ReadOnlyListProxy<T>(l);
	}

	public static ISet<T> ReadOnly<T>(ISet<T> s)
	{
		return new ReadOnlySetProxy<T>(s);
	}

	public static bool Remove<K, V>(IDictionary<K, V> d, K k, out V v)
	{
		if (!d.TryGetValue(k, out v))
		{
			return false;
		}
		d.Remove(k);
		return true;
	}

	public static T RequireNext<T>(IEnumerator<T> e)
	{
		if (!e.MoveNext())
		{
			throw new InvalidOperationException();
		}
		return e.Current;
	}

	public static string ToString<T>(IEnumerable<T> c)
	{
		IEnumerator<T> enumerator = c.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder("[");
		stringBuilder.Append(enumerator.Current);
		while (enumerator.MoveNext())
		{
			stringBuilder.Append(", ");
			stringBuilder.Append(enumerator.Current);
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
}
