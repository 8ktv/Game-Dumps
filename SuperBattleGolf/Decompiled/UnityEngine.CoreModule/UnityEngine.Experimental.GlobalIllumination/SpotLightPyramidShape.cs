using System;

namespace UnityEngine.Experimental.GlobalIllumination;

public struct SpotLightPyramidShape
{
	public EntityId entityId;

	public bool shadow;

	public LightMode mode;

	public Vector3 position;

	public Quaternion orientation;

	public LinearColor color;

	public LinearColor indirectColor;

	public float range;

	public float angle;

	public float aspectRatio;

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
