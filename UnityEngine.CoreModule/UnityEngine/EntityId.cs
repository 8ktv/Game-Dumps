using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential, Size = 4)]
[UsedByNativeCode]
[NativeClass("EntityId")]
public struct EntityId : IEquatable<EntityId>, IComparable<EntityId>
{
	[SerializeField]
	private int m_Data;

	public static EntityId None => default(EntityId);

	public override bool Equals(object obj)
	{
		return obj is EntityId other && Equals(other);
	}

	public bool Equals(EntityId other)
	{
		return m_Data == other.m_Data;
	}

	public int CompareTo(EntityId other)
	{
		return m_Data.CompareTo(other.m_Data);
	}

	public static bool operator ==(EntityId left, EntityId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(EntityId left, EntityId right)
	{
		return !left.Equals(right);
	}

	public static bool operator <(EntityId left, EntityId right)
	{
		return left.m_Data < right.m_Data;
	}

	public static bool operator >(EntityId left, EntityId right)
	{
		return left.m_Data > right.m_Data;
	}

	public static bool operator <=(EntityId left, EntityId right)
	{
		return left.m_Data <= right.m_Data;
	}

	public static bool operator >=(EntityId left, EntityId right)
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

	public static implicit operator int(EntityId entityId)
	{
		return entityId.m_Data;
	}

	public static implicit operator EntityId(int intValue)
	{
		return new EntityId
		{
			m_Data = intValue
		};
	}

	public static implicit operator EntityId(InstanceID entityId)
	{
		return new EntityId
		{
			m_Data = entityId
		};
	}

	public static implicit operator InstanceID(EntityId entityId)
	{
		return (int)entityId;
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
