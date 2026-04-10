namespace UnityEngine;

public struct ContactPoint
{
	internal Vector3 m_Point;

	internal Vector3 m_Normal;

	internal Vector3 m_Impulse;

	internal EntityId m_ThisColliderEntityId;

	internal EntityId m_OtherColliderEntityId;

	internal float m_Separation;

	public Vector3 point => m_Point;

	public Vector3 normal => m_Normal;

	public Vector3 impulse => m_Impulse;

	public Collider thisCollider => Physics.GetColliderByInstanceID(m_ThisColliderEntityId);

	public Collider otherCollider => Physics.GetColliderByInstanceID(m_OtherColliderEntityId);

	public float separation => m_Separation;

	internal ContactPoint(Vector3 point, Vector3 normal, Vector3 impulse, float separation, EntityId thisEntityId, EntityId otherEntityId)
	{
		m_Point = point;
		m_Normal = normal;
		m_Impulse = impulse;
		m_Separation = separation;
		m_ThisColliderEntityId = thisEntityId;
		m_OtherColliderEntityId = otherEntityId;
	}
}
