using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal readonly struct TypeConverterRegistry : IEqualityComparer<TypeConverterRegistry>
{
	private class ConverterKeyComparer : IEqualityComparer<ConverterKey>
	{
		public bool Equals(ConverterKey x, ConverterKey y)
		{
			return x.SourceType == y.SourceType && x.DestinationType == y.DestinationType;
		}

		public int GetHashCode(ConverterKey obj)
		{
			return (((obj.SourceType != null) ? obj.SourceType.GetHashCode() : 0) * 397) ^ ((obj.DestinationType != null) ? obj.DestinationType.GetHashCode() : 0);
		}
	}

	private readonly struct ConverterKey
	{
		public readonly Type SourceType;

		public readonly Type DestinationType;

		public ConverterKey(Type source, Type destination)
		{
			SourceType = source;
			DestinationType = destination;
		}
	}

	private static readonly ConverterKeyComparer k_Comparer = new ConverterKeyComparer();

	private readonly Dictionary<ConverterKey, Delegate> m_Converters;

	public int ConverterCount => m_Converters?.Count ?? 0;

	private TypeConverterRegistry(Dictionary<ConverterKey, Delegate> storage)
	{
		m_Converters = storage;
	}

	public static TypeConverterRegistry Create()
	{
		return new TypeConverterRegistry(new Dictionary<ConverterKey, Delegate>(k_Comparer));
	}

	public void Register(Type source, Type destination, Delegate converter)
	{
		m_Converters[new ConverterKey(source, destination)] = converter ?? throw new ArgumentException("converter");
	}

	public void Unregister(Type source, Type destination)
	{
		m_Converters.Remove(new ConverterKey(source, destination));
	}

	internal void Apply(TypeConverterRegistry registry)
	{
		foreach (KeyValuePair<ConverterKey, Delegate> converter in registry.m_Converters)
		{
			Register(converter.Key.SourceType, converter.Key.DestinationType, converter.Value);
		}
	}

	public Delegate GetConverter(Type source, Type destination)
	{
		ConverterKey key = new ConverterKey(source, destination);
		Delegate value;
		return m_Converters.TryGetValue(key, out value) ? value : null;
	}

	public bool TryGetConverter(Type source, Type destination, out Delegate converter)
	{
		converter = GetConverter(source, destination);
		return (object)converter != null;
	}

	public void GetAllTypesConvertingToType(Type type, List<Type> result)
	{
		if (m_Converters == null)
		{
			return;
		}
		foreach (ConverterKey key in m_Converters.Keys)
		{
			if (key.DestinationType == type)
			{
				result.Add(key.SourceType);
			}
		}
	}

	public void GetAllTypesConvertingFromType(Type type, List<Type> result)
	{
		if (m_Converters == null)
		{
			return;
		}
		foreach (ConverterKey key in m_Converters.Keys)
		{
			if (key.SourceType == type)
			{
				result.Add(key.DestinationType);
			}
		}
	}

	public void GetAllConversions(List<(Type, Type)> result)
	{
		if (m_Converters == null)
		{
			return;
		}
		foreach (ConverterKey key in m_Converters.Keys)
		{
			result.Add((key.SourceType, key.DestinationType));
		}
	}

	public bool Equals(TypeConverterRegistry x, TypeConverterRegistry y)
	{
		return x.m_Converters == y.m_Converters;
	}

	public int GetHashCode(TypeConverterRegistry obj)
	{
		return (obj.m_Converters != null) ? obj.m_Converters.GetHashCode() : 0;
	}
}
