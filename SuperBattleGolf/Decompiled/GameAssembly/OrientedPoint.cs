using UnityEngine;

public class OrientedPoint
{
	public Vector3 position;

	public Quaternion rotation;

	public OrientedPoint(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}
}
