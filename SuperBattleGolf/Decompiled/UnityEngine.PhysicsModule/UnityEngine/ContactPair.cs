using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public readonly struct ContactPair
{
	private const uint c_InvalidFaceIndex = uint.MaxValue;

	internal readonly EntityId m_ColliderID;

	internal readonly EntityId m_OtherColliderID;

	internal readonly IntPtr m_StartPtr;

	internal readonly uint m_NbPoints;

	internal readonly CollisionPairFlags m_Flags;

	internal readonly CollisionPairEventFlags m_Events;

	internal readonly Vector3 m_ImpulseSum;

	[Obsolete("colliderInstanceID is deprecated, use colliderEntityId instead.", false)]
	public int colliderInstanceID => m_ColliderID;

	[Obsolete("otherColliderInstanceID is deprecated, use otherColliderEntityId instead.", false)]
	public int otherColliderInstanceID => m_OtherColliderID;

	public EntityId colliderEntityId => m_ColliderID;

	public EntityId otherColliderEntityId => m_OtherColliderID;

	public Collider collider => (m_ColliderID == 0) ? null : Physics.GetColliderByInstanceID(m_ColliderID);

	public Collider otherCollider => (m_OtherColliderID == 0) ? null : Physics.GetColliderByInstanceID(m_OtherColliderID);

	public int contactCount => (int)m_NbPoints;

	public Vector3 impulseSum => m_ImpulseSum;

	public bool isCollisionEnter => (m_Events & CollisionPairEventFlags.NotifyTouchFound) != 0;

	public bool isCollisionExit => (m_Events & CollisionPairEventFlags.NotifyTouchLost) != 0;

	public bool isCollisionStay => (m_Events & CollisionPairEventFlags.NotifyTouchPersists) != 0;

	internal bool hasRemovedCollider => (m_Flags & CollisionPairFlags.RemovedShape) != 0 || (m_Flags & CollisionPairFlags.RemovedOtherShape) != 0;

	[Obsolete("Please use ContactPair.colliderInstanceID instead. (UnityUpgradable) -> colliderInstanceID", false)]
	public int ColliderInstanceID => colliderInstanceID;

	[Obsolete("Please use ContactPair.otherColliderInstanceID instead. (UnityUpgradable) -> otherColliderInstanceID", false)]
	public int OtherColliderInstanceID => otherColliderInstanceID;

	[Obsolete("Please use ContactPair.collider instead. (UnityUpgradable) -> collider", false)]
	public Collider Collider => collider;

	[Obsolete("Please use ContactPair.otherCollider instead. (UnityUpgradable) -> otherCollider", false)]
	public Collider OtherCollider => otherCollider;

	[Obsolete("Please use ContactPair.contactCount instead. (UnityUpgradable) -> contactCount", false)]
	public int ContactCount => contactCount;

	[Obsolete("Please use ContactPair.impulseSum instead. (UnityUpgradable) -> impulseSum", false)]
	public Vector3 ImpulseSum => impulseSum;

	[Obsolete("Please use ContactPair.isCollisionEnter instead. (UnityUpgradable) -> isCollisionEnter", false)]
	public bool IsCollisionEnter => isCollisionEnter;

	[Obsolete("Please use ContactPair.isCollisionExit instead. (UnityUpgradable) -> isCollisionExit", false)]
	public bool IsCollisionExit => isCollisionExit;

	[Obsolete("Please use ContactPair.isCollisionStay instead. (UnityUpgradable) -> isCollisionStay", false)]
	public bool IsCollisionStay => isCollisionStay;

	internal int ExtractContacts(List<ContactPoint> managedContainer, bool flipped)
	{
		int num = (int)Math.Min(managedContainer.Capacity, m_NbPoints);
		managedContainer.Clear();
		for (int i = 0; i < num; i++)
		{
			ref readonly ContactPairPoint contactPoint = ref GetContactPoint(i);
			ContactPoint item = new ContactPoint
			{
				m_Point = contactPoint.position,
				m_Impulse = contactPoint.impulse,
				m_Separation = contactPoint.separation
			};
			if (flipped)
			{
				item.m_Normal = -contactPoint.normal;
				item.m_ThisColliderEntityId = m_OtherColliderID;
				item.m_OtherColliderEntityId = m_ColliderID;
			}
			else
			{
				item.m_Normal = contactPoint.normal;
				item.m_ThisColliderEntityId = m_ColliderID;
				item.m_OtherColliderEntityId = m_OtherColliderID;
			}
			managedContainer.Add(item);
		}
		return num;
	}

	internal int ExtractContactsArray(ContactPoint[] managedContainer, bool flipped)
	{
		int num = (int)Math.Min(managedContainer.Length, m_NbPoints);
		for (int i = 0; i < num; i++)
		{
			ref readonly ContactPairPoint contactPoint = ref GetContactPoint(i);
			ContactPoint contactPoint2 = new ContactPoint
			{
				m_Point = contactPoint.position,
				m_Impulse = contactPoint.impulse,
				m_Separation = contactPoint.separation
			};
			if (flipped)
			{
				contactPoint2.m_Normal = -contactPoint.normal;
				contactPoint2.m_ThisColliderEntityId = m_OtherColliderID;
				contactPoint2.m_OtherColliderEntityId = m_ColliderID;
			}
			else
			{
				contactPoint2.m_Normal = contactPoint.normal;
				contactPoint2.m_ThisColliderEntityId = m_ColliderID;
				contactPoint2.m_OtherColliderEntityId = m_OtherColliderID;
			}
			managedContainer[i] = contactPoint2;
		}
		return num;
	}

	public void CopyToNativeArray(NativeArray<ContactPairPoint> buffer)
	{
		int num = Mathf.Min(buffer.Length, contactCount);
		for (int i = 0; i < num; i++)
		{
			buffer[i] = GetContactPoint(i);
		}
	}

	public unsafe ref readonly ContactPairPoint GetContactPoint(int index)
	{
		return ref *GetContactPoint_Internal(index);
	}

	public unsafe uint GetContactPointFaceIndex(int contactIndex)
	{
		uint internalFaceIndex = GetContactPoint_Internal(contactIndex)->m_InternalFaceIndex0;
		uint internalFaceIndex2 = GetContactPoint_Internal(contactIndex)->m_InternalFaceIndex1;
		if (internalFaceIndex != uint.MaxValue)
		{
			return Physics.TranslateTriangleIndexFromID(m_ColliderID, internalFaceIndex);
		}
		if (internalFaceIndex2 != uint.MaxValue)
		{
			return Physics.TranslateTriangleIndexFromID(m_OtherColliderID, internalFaceIndex2);
		}
		return uint.MaxValue;
	}

	internal unsafe ContactPairPoint* GetContactPoint_Internal(int index)
	{
		if (index >= m_NbPoints)
		{
			throw new IndexOutOfRangeException("Invalid ContactPairPoint index. Index should be greater than 0 and less than ContactPair.ContactCount");
		}
		return (ContactPairPoint*)(m_StartPtr.ToInt64() + index * sizeof(ContactPairPoint));
	}
}
