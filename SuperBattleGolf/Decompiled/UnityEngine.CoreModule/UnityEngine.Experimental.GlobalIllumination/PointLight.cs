using System;

namespace UnityEngine.Experimental.GlobalIllumination;

public struct PointLight
{
	public EntityId entityId;

	public bool shadow;

	public LightMode mode;

	public Vector3 position;

	public Quaternion orientation;

	public LinearColor color;

	public LinearColor indirectColor;

	public float range;

	public float sphereRadius;

	public FalloffType falloff;

	[Obsolete("Please use entityId instead.", false)]
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
}
