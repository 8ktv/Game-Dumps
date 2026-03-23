using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public struct GolfCartState : PredictedState
{
	public double timestamp
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private set;
	}

	public Vector3 positionDelta
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Vector3 position
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Quaternion rotationDelta
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Quaternion rotation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Vector3 velocityDelta
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Vector3 velocity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Vector3 angularVelocityDelta
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Vector3 angularVelocity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Vector4 wheelSpeedsDelta
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public Vector4 wheelSpeeds
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set;
	}

	public GolfCartState(double timestamp, Vector3 positionDelta, Vector3 position, Quaternion rotationDelta, Quaternion rotation, Vector3 velocityDelta, Vector3 velocity, Vector3 angularVelocityDelta, Vector3 angularVelocity, Vector4 wheelSpeedsDelta, Vector4 wheelSpeeds)
	{
		this.timestamp = timestamp;
		this.positionDelta = positionDelta;
		this.position = position;
		this.rotationDelta = rotationDelta;
		this.rotation = rotation;
		this.velocityDelta = velocityDelta;
		this.velocity = velocity;
		this.angularVelocityDelta = angularVelocityDelta;
		this.angularVelocity = angularVelocity;
		this.wheelSpeedsDelta = wheelSpeedsDelta;
		this.wheelSpeeds = wheelSpeeds;
	}

	public static GolfCartState Interpolate(GolfCartState a, GolfCartState b, float t)
	{
		return new GolfCartState
		{
			position = Vector3.Lerp(a.position, b.position, t),
			rotation = Quaternion.Slerp(a.rotation, b.rotation, t).normalized,
			velocity = Vector3.Lerp(a.velocity, b.velocity, t),
			angularVelocity = Vector3.Lerp(a.angularVelocity, b.angularVelocity, t),
			wheelSpeeds = Vector4.Lerp(a.wheelSpeeds, b.wheelSpeeds, t)
		};
	}
}
