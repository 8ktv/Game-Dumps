using UnityEngine;

namespace Mirror;

public struct PredictedSyncData
{
	public float deltaTime;

	public Vector3 position;

	public Quaternion rotation;

	public Vector3 velocity;

	public Vector3 angularVelocity;

	public PredictedSyncData(float deltaTime, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		this.deltaTime = deltaTime;
		this.position = position;
		this.rotation = rotation;
		this.velocity = velocity;
		this.angularVelocity = angularVelocity;
	}
}
