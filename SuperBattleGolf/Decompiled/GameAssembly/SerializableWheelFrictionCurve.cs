using System;
using UnityEngine;

[Serializable]
public struct SerializableWheelFrictionCurve
{
	public float extremumSlip;

	public float extremumValue;

	public float asymptoteSlip;

	public float asymptoteValue;

	public float stiffness;

	public static implicit operator WheelFrictionCurve(SerializableWheelFrictionCurve friction)
	{
		return new WheelFrictionCurve
		{
			extremumSlip = friction.extremumSlip,
			extremumValue = friction.extremumValue,
			asymptoteSlip = friction.asymptoteSlip,
			asymptoteValue = friction.asymptoteValue,
			stiffness = friction.stiffness
		};
	}
}
