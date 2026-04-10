using System;

namespace UnityEngine;

[Serializable]
public struct LazyLoadReference<T> where T : Object
{
	[SerializeField]
	private int m_InstanceID;

	public bool isSet => m_InstanceID != EntityId.None;

	public bool isBroken => m_InstanceID != EntityId.None && !Object.DoesObjectWithInstanceIDExist(m_InstanceID);

	public T asset
	{
		get
		{
			if (m_InstanceID == EntityId.None)
			{
				return null;
			}
			return (T)Object.ForceLoadFromInstanceID(m_InstanceID);
		}
		set
		{
			if (value == null)
			{
				m_InstanceID = EntityId.None;
				return;
			}
			if (!Object.IsPersistent(value))
			{
				throw new ArgumentException("Object that does not belong to a persisted asset cannot be set as the target of a LazyLoadReference.");
			}
			m_InstanceID = value.GetEntityId();
		}
	}

	public EntityId entityId
	{
		get
		{
			return m_InstanceID;
		}
		set
		{
			m_InstanceID = value;
		}
	}

	[Obsolete("Use entityId instead, this will be removed in a future version", false)]
	public int instanceID
	{
		get
		{
			return entityId;
		}
		set
		{
			entityId = value;
		}
	}

	public LazyLoadReference(T asset)
	{
		if (asset == null)
		{
			m_InstanceID = EntityId.None;
			return;
		}
		if (!Object.IsPersistent(asset))
		{
			throw new ArgumentException("Object that does not belong to a persisted asset cannot be set as the target of a LazyLoadReference.");
		}
		m_InstanceID = asset.GetEntityId();
	}

	public LazyLoadReference(EntityId entityId)
	{
		m_InstanceID = entityId;
	}

	[Obsolete("Use LazyLoadReference(EntityId entityId) instead, this will be removed in a future version", false)]
	public LazyLoadReference(int instanceID)
	{
		m_InstanceID = instanceID;
	}

	public static implicit operator LazyLoadReference<T>(T asset)
	{
		return new LazyLoadReference<T>
		{
			asset = asset
		};
	}

	public static implicit operator LazyLoadReference<T>(EntityId entityId)
	{
		return new LazyLoadReference<T>
		{
			m_InstanceID = entityId
		};
	}

	[Obsolete("Use LazyLoadReference(EntityId entityId) instead, this will be removed in a future version", false)]
	public static implicit operator LazyLoadReference<T>(int instanceID)
	{
		return new LazyLoadReference<T>
		{
			m_InstanceID = instanceID
		};
	}
}
