using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public readonly struct ContactPairHeader
{
	internal readonly EntityId m_BodyID;

	internal readonly EntityId m_OtherBodyID;

	internal readonly IntPtr m_StartPtr;

	internal readonly uint m_NbPairs;

	internal readonly CollisionPairHeaderFlags m_Flags;

	internal readonly Vector3 m_RelativeVelocity;

	[Obsolete("bodyInstanceID is deprecated, use bodyEntityId instead.", false)]
	public int bodyInstanceID => m_BodyID;

	[Obsolete("otherBodyInstanceID is deprecated, use otherBodyEntityId instead.", false)]
	public int otherBodyInstanceID => m_OtherBodyID;

	public EntityId bodyEntityId => m_BodyID;

	public EntityId otherBodyEntityId => m_OtherBodyID;

	public Component body => Physics.GetBodyByInstanceID(m_BodyID);

	public Component otherBody => Physics.GetBodyByInstanceID(m_OtherBodyID);

	public int pairCount => (int)m_NbPairs;

	internal bool hasRemovedBody => (m_Flags & CollisionPairHeaderFlags.RemovedActor) != 0 || (m_Flags & CollisionPairHeaderFlags.RemovedOtherActor) != 0;

	[Obsolete("Please use ContactPairHeader.bodyInstanceID instead. (UnityUpgradable) -> bodyInstanceID", false)]
	public int BodyInstanceID => bodyInstanceID;

	[Obsolete("Please use ContactPairHeader.otherBodyInstanceID instead. (UnityUpgradable) -> otherBodyInstanceID", false)]
	public int OtherBodyInstanceID => otherBodyInstanceID;

	[Obsolete("Please use ContactPairHeader.body instead. (UnityUpgradable) -> body", false)]
	public Component Body => body;

	[Obsolete("Please use ContactPairHeader.otherBody instead. (UnityUpgradable) -> otherBody", false)]
	public Component OtherBody => otherBody;

	[Obsolete("Please use ContactPairHeader.pairCount instead. (UnityUpgradable) -> pairCount", false)]
	public int PairCount => pairCount;

	public unsafe ref readonly ContactPair GetContactPair(int index)
	{
		return ref *GetContactPair_Internal(index);
	}

	internal unsafe ContactPair* GetContactPair_Internal(int index)
	{
		if (index >= m_NbPairs)
		{
			throw new IndexOutOfRangeException("Invalid ContactPair index. Index should be greater than 0 and less than ContactPairHeader.PairCount");
		}
		return (ContactPair*)(m_StartPtr.ToInt64() + index * sizeof(ContactPair));
	}
}
