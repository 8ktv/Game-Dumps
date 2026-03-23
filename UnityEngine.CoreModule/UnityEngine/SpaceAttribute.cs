using System;

namespace UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class SpaceAttribute : PropertyAttribute
{
	public readonly float height;

	public SpaceAttribute()
		: base(applyToCollection: true)
	{
		height = 8f;
	}

	public SpaceAttribute(float height)
		: base(applyToCollection: true)
	{
		this.height = height;
	}
}
