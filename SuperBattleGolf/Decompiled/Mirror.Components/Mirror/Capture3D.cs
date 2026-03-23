using UnityEngine;

namespace Mirror;

public struct Capture3D : Capture
{
	public Vector3 position;

	public Vector3 size;

	public double timestamp { get; set; }

	public Capture3D(double timestamp, Vector3 position, Vector3 size)
	{
		this.timestamp = timestamp;
		this.position = position;
		this.size = size;
	}

	public void DrawGizmo()
	{
		Gizmos.DrawWireCube(position, size);
	}

	public static Capture3D Interpolate(Capture3D from, Capture3D to, double t)
	{
		return new Capture3D(0.0, Vector3.LerpUnclamped(from.position, to.position, (float)t), Vector3.LerpUnclamped(from.size, to.size, (float)t));
	}

	public override string ToString()
	{
		return $"(time={timestamp} pos={position} size={size})";
	}
}
