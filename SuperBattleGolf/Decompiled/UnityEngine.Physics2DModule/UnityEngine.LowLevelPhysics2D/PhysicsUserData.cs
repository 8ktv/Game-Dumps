using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsUserData
{
	[SerializeField]
	internal EntityId m_EntityId;

	[SerializeField]
	internal PhysicsMask m_PhysicsMask;

	[SerializeField]
	internal float m_Float;

	[SerializeField]
	internal int m_Int;

	[SerializeField]
	internal ulong m_Int64;

	[SerializeField]
	internal bool m_Bool;

	public Object objectValue
	{
		readonly get
		{
			return PhysicsLowLevelScripting2D.PhysicsUserData_GetObject(m_EntityId);
		}
		set
		{
			m_EntityId = ((value != null) ? value.GetEntityId() : EntityId.None);
		}
	}

	public PhysicsMask physicsMaskValue
	{
		readonly get
		{
			return m_PhysicsMask;
		}
		set
		{
			m_PhysicsMask = value;
		}
	}

	public float floatValue
	{
		readonly get
		{
			return m_Float;
		}
		set
		{
			m_Float = value;
		}
	}

	public int intValue
	{
		readonly get
		{
			return m_Int;
		}
		set
		{
			m_Int = value;
		}
	}

	public ulong int64Value
	{
		readonly get
		{
			return m_Int64;
		}
		set
		{
			m_Int64 = value;
		}
	}

	public bool boolValue
	{
		readonly get
		{
			return m_Bool;
		}
		set
		{
			m_Bool = value;
		}
	}

	public override readonly string ToString()
	{
		return $"object={objectValue}, physicsMask={physicsMaskValue}, float={floatValue}, int={intValue}, int64={int64Value}, bool={boolValue}";
	}
}
