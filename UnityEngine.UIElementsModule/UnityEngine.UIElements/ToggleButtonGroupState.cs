using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
public struct ToggleButtonGroupState : IEquatable<ToggleButtonGroupState>, IComparable<ToggleButtonGroupState>
{
	internal const int maxLength = 64;

	[SerializeField]
	private ulong m_Data;

	[SerializeField]
	private int m_Length;

	public int length
	{
		get
		{
			return m_Length;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Length = value;
		}
	}

	internal ulong data => m_Data;

	public bool this[int index]
	{
		get
		{
			if (index < 0 || index >= m_Length)
			{
				throw new ArgumentOutOfRangeException("index", $"index of {index} should be in the range of 0 and {m_Length - 1} inclusively.");
			}
			ulong num = (ulong)(1L << index);
			return (m_Data & num) == num;
		}
		set
		{
			if (index < 0 || index >= m_Length)
			{
				throw new ArgumentOutOfRangeException("index", $"index of {index} should be in the range of 0 and {m_Length - 1} inclusively.");
			}
			ulong num = (ulong)(1L << index);
			if (value)
			{
				m_Data |= num;
			}
			else
			{
				m_Data &= ~num;
			}
		}
	}

	public ToggleButtonGroupState(ulong optionsBitMask, int length)
	{
		if (length < 0 || length > 64)
		{
			throw new ArgumentOutOfRangeException("length", $"length of {length} should be greater than or equal to 0 and less than or equal to {64}.");
		}
		m_Data = optionsBitMask;
		m_Length = length;
		ResetOptions(m_Length);
	}

	public Span<int> GetActiveOptions(Span<int> activeOptionsIndices)
	{
		if (activeOptionsIndices.Length < m_Length)
		{
			throw new ArgumentException($"indices' length ({activeOptionsIndices.Length}) should be equal to or greater than the ToggleButtonGroupState's length ({m_Length}).");
		}
		int num = 0;
		for (int i = 0; i < m_Length; i++)
		{
			if (this[i])
			{
				activeOptionsIndices[num] = i;
				num++;
			}
		}
		return activeOptionsIndices.Slice(0, num);
	}

	public Span<int> GetInactiveOptions(Span<int> inactiveOptionsIndices)
	{
		if (inactiveOptionsIndices.Length < m_Length)
		{
			throw new ArgumentException($"indices' length ({inactiveOptionsIndices.Length}) should be equal to or greater than the ToggleButtonGroupState's length ({m_Length}).");
		}
		int num = 0;
		for (int i = 0; i < m_Length; i++)
		{
			if (!this[i])
			{
				inactiveOptionsIndices[num] = i;
				num++;
			}
		}
		return inactiveOptionsIndices.Slice(0, num);
	}

	public void SetAllOptions()
	{
		m_Data = ulong.MaxValue;
		ResetOptions(m_Length);
	}

	public void ResetAllOptions()
	{
		m_Data = 0uL;
	}

	public void ToggleAllOptions()
	{
		m_Data = ~m_Data;
		ResetOptions(m_Length);
	}

	public static ToggleButtonGroupState CreateFromOptions(IList<bool> options)
	{
		int count = options.Count;
		ToggleButtonGroupState result = new ToggleButtonGroupState(0uL, count);
		for (int i = 0; i < count; i++)
		{
			result[i] = options[i];
		}
		return result;
	}

	public static ToggleButtonGroupState FromEnumFlags<T>(T options, int length = -1) where T : Enum
	{
		if (!TypeTraits<T>.IsEnumFlags)
		{
			throw new ArgumentException("Enum type T is not a flag enum type.");
		}
		Type underlyingType = Enum.GetUnderlyingType(typeof(T));
		if (length == -1)
		{
			TypeCode typeCode = Type.GetTypeCode(underlyingType);
			if (1 == 0)
			{
			}
			int num = typeCode switch
			{
				TypeCode.Byte => 8, 
				TypeCode.SByte => 8, 
				TypeCode.UInt16 => 16, 
				TypeCode.Int16 => 16, 
				TypeCode.UInt32 => 32, 
				TypeCode.Int32 => 32, 
				TypeCode.Int64 => 64, 
				TypeCode.UInt64 => 64, 
				_ => 0, 
			};
			if (1 == 0)
			{
			}
			length = num;
		}
		return new ToggleButtonGroupState((ulong)UnsafeUtility.As<T, int>(ref options), length);
	}

	public static T ToEnumFlags<T>(ToggleButtonGroupState options, bool acceptsLengthMismatch = true) where T : Enum
	{
		if (!TypeTraits<T>.IsEnumFlags)
		{
			throw new ArgumentException("Enum type T is not a flag enum type.");
		}
		Type underlyingType = Enum.GetUnderlyingType(typeof(T));
		TypeCode typeCode = Type.GetTypeCode(underlyingType);
		if (1 == 0)
		{
		}
		int num = typeCode switch
		{
			TypeCode.Byte => 8, 
			TypeCode.SByte => 8, 
			TypeCode.UInt16 => 16, 
			TypeCode.Int16 => 16, 
			TypeCode.UInt32 => 32, 
			TypeCode.Int32 => 32, 
			TypeCode.Int64 => 64, 
			TypeCode.UInt64 => 64, 
			_ => -1, 
		};
		if (1 == 0)
		{
		}
		int num2 = num;
		if (!acceptsLengthMismatch && options.m_Length != num2)
		{
			throw new ArgumentException("Cannot sync to enum flag since the ToggleButtonGroupState has a different amount of options.");
		}
		return (T)Enum.Parse(typeof(T), options.m_Data.ToString());
	}

	public int CompareTo(ToggleButtonGroupState other)
	{
		return (other == this) ? 1 : (-1);
	}

	public static bool Compare<T>(ToggleButtonGroupState options, T value) where T : Enum
	{
		if (!TypeTraits<T>.IsEnumFlags)
		{
			throw new ArgumentException("Enum type T is not a flag enum type.");
		}
		ulong num = (ulong)UnsafeUtility.As<T, int>(ref value);
		return options.m_Data == num;
	}

	private void ResetOptions(int startingIndex)
	{
		for (int i = startingIndex; i < 64; i++)
		{
			ulong num = (ulong)(1L << i);
			m_Data &= ~num;
		}
	}

	public static bool operator ==(ToggleButtonGroupState lhs, ToggleButtonGroupState rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(ToggleButtonGroupState lhs, ToggleButtonGroupState rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(ToggleButtonGroupState other)
	{
		return m_Data == other.m_Data && m_Length == other.m_Length;
	}

	public override bool Equals(object obj)
	{
		return obj is ToggleButtonGroupState other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(m_Data, m_Length);
	}

	public override string ToString()
	{
		return Convert.ToString((long)m_Data, 2).PadLeft(length, '0');
	}
}
