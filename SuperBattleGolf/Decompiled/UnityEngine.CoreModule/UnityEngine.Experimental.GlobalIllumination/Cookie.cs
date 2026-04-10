using System;

namespace UnityEngine.Experimental.GlobalIllumination;

public struct Cookie
{
	public EntityId entityId;

	public float scale;

	public Vector2 sizes;

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

	public static Cookie Defaults()
	{
		Cookie result = default(Cookie);
		result.entityId = EntityId.None;
		result.scale = 1f;
		result.sizes = new Vector2(1f, 1f);
		return result;
	}
}
