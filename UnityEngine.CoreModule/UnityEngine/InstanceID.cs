using System;
using System.Runtime.InteropServices;

namespace UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential, Size = 4)]
[Obsolete("Obsolete - Please use EntityId instead.")]
public struct InstanceID : IEquatable<InstanceID>, IComparable<InstanceID>
{
	[SerializeField]
	private int m_Data;

	public static InstanceID None => default(InstanceID);

	public override bool Equals(object obj)
	{
		return obj is InstanceID other && Equals(other);
	}

	public bool Equals(InstanceID other)
	{
		return m_Data == other.m_Data;
	}

	public int CompareTo(InstanceID other)
	{
		return m_Data.CompareTo(other.m_Data);
	}

	public static bool operator ==(InstanceID left, InstanceID right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(InstanceID left, InstanceID right)
	{
		return !left.Equals(right);
	}

	public static bool operator <(InstanceID left, InstanceID right)
	{
		return left.m_Data < right.m_Data;
	}

	public static bool operator >(InstanceID left, InstanceID right)
	{
		return left.m_Data > right.m_Data;
	}

	public static bool operator <=(InstanceID left, InstanceID right)
	{
		return left.m_Data <= right.m_Data;
	}

	public static bool operator >=(InstanceID left, InstanceID right)
	{
		return left.m_Data >= right.m_Data;
	}

	public override int GetHashCode()
	{
		uint data = (uint)m_Data;
		data = data + 2127912214 + (data << 12);
		data = data ^ 0xC761C23Cu ^ (data >> 19);
		data = data + 374761393 + (data << 5);
		data = (uint)((int)data + -744332180) ^ (data << 9);
		data = (uint)((int)data + -42973499) + (data << 3);
		return (int)(data ^ 0xB55A4F09u ^ (data >> 16));
	}

	public bool IsValid()
	{
		return this != None;
	}

	public bool Equals(int other)
	{
		return m_Data == other;
	}

	public static implicit operator int(InstanceID entityId)
	{
		return entityId.m_Data;
	}

	public static implicit operator InstanceID(int intValue)
	{
		return new InstanceID
		{
			m_Data = intValue
		};
	}

	public static implicit operator EntityId(InstanceID entityId)
	{
		return (int)entityId;
	}

	public static implicit operator InstanceID(EntityId entityId)
	{
		return new InstanceID
		{
			m_Data = entityId
		};
	}

	public override string ToString()
	{
		return m_Data.ToString();
	}

	public string ToString(string format)
	{
		return m_Data.ToString(format);
	}
}
