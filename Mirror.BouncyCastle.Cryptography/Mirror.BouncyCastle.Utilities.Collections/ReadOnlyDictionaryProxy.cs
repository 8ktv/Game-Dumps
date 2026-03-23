using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal class ReadOnlyDictionaryProxy<K, V> : ReadOnlyDictionary<K, V>
{
	private readonly IDictionary<K, V> m_target;

	public override int Count => m_target.Count;

	public override ICollection<K> Keys => new ReadOnlyCollectionProxy<K>(m_target.Keys);

	public override ICollection<V> Values => new ReadOnlyCollectionProxy<V>(m_target.Values);

	internal ReadOnlyDictionaryProxy(IDictionary<K, V> target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		m_target = target;
	}

	public override bool Contains(KeyValuePair<K, V> item)
	{
		return m_target.Contains(item);
	}

	public override bool ContainsKey(K key)
	{
		return m_target.ContainsKey(key);
	}

	public override void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
	{
		m_target.CopyTo(array, arrayIndex);
	}

	public override IEnumerator<KeyValuePair<K, V>> GetEnumerator()
	{
		return m_target.GetEnumerator();
	}

	public override bool TryGetValue(K key, out V value)
	{
		return m_target.TryGetValue(key, out value);
	}

	protected override V Lookup(K key)
	{
		return m_target[key];
	}
}
