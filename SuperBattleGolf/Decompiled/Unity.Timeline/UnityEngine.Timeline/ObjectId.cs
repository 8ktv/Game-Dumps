using System;

namespace UnityEngine.Timeline;

[Serializable]
internal struct ObjectId : IEquatable<ObjectId>, IComparable<ObjectId>
{
	public static readonly ObjectId InvalidId = new ObjectId(-1);

	public static readonly ObjectId DefaultId = new ObjectId(0);

	[SerializeField]
	private int m_Data;

	internal ObjectId(int data)
	{
		m_Data = data;
	}

	public static implicit operator ObjectId(EntityId entityId)
	{
		return new ObjectId
		{
			m_Data = entityId
		};
	}

	public static implicit operator EntityId(ObjectId objectId)
	{
		return objectId.m_Data;
	}

	public override bool Equals(object obj)
	{
		if (obj is ObjectId other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(ObjectId other)
	{
		return m_Data == other.m_Data;
	}

	public int CompareTo(ObjectId other)
	{
		return m_Data.CompareTo(other.m_Data);
	}

	public static bool operator ==(ObjectId left, ObjectId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ObjectId left, ObjectId right)
	{
		return !left.Equals(right);
	}

	public static bool operator <(ObjectId left, ObjectId right)
	{
		return left.m_Data < right.m_Data;
	}

	public static bool operator >(ObjectId left, ObjectId right)
	{
		return left.m_Data > right.m_Data;
	}

	public static bool operator <=(ObjectId left, ObjectId right)
	{
		return left.m_Data <= right.m_Data;
	}

	public static bool operator >=(ObjectId left, ObjectId right)
	{
		return left.m_Data >= right.m_Data;
	}

	public override int GetHashCode()
	{
		return m_Data;
	}
}
