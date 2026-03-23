using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/ Lag Compensation/ Lag Compensator")]
[HelpURL("https://mirror-networking.gitbook.io/docs/manual/general/lag-compensation")]
public class LagCompensator : NetworkBehaviour
{
	[Header("Components")]
	[Tooltip("The collider to keep a history of.")]
	public Collider trackedCollider;

	[Header("Settings")]
	public LagCompensationSettings lagCompensationSettings = new LagCompensationSettings();

	private double lastCaptureTime;

	private readonly Queue<KeyValuePair<double, Capture3D>> history = new Queue<KeyValuePair<double, Capture3D>>();

	[Header("Debugging")]
	public Color historyColor = Color.white;

	[ServerCallback]
	protected virtual void Update()
	{
		if (NetworkServer.active && NetworkTime.localTime >= lastCaptureTime + (double)lagCompensationSettings.captureInterval)
		{
			lastCaptureTime = NetworkTime.localTime;
			Capture();
		}
	}

	[ServerCallback]
	protected virtual void Capture()
	{
		if (NetworkServer.active)
		{
			Capture3D capture = new Capture3D(NetworkTime.localTime, trackedCollider.bounds.center, trackedCollider.bounds.size);
			LagCompensation.Insert(history, lagCompensationSettings.historyLimit, NetworkTime.localTime, capture);
		}
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = historyColor;
		LagCompensation.DrawGizmos(history);
	}

	[ServerCallback]
	public virtual bool Sample(NetworkConnectionToClient viewer, out Capture3D sample)
	{
		if (!NetworkServer.active)
		{
			sample = default(Capture3D);
			return default(bool);
		}
		double num = LagCompensation.EstimateTime(NetworkTime.localTime, viewer.rtt, NetworkClient.bufferTime);
		if (LagCompensation.Sample(history, num, lagCompensationSettings.captureInterval, out var before, out var after, out var t))
		{
			sample = Capture3D.Interpolate(before, after, t);
			return true;
		}
		Debug.Log($"CmdClicked: history doesn't contain {num:F3}");
		sample = default(Capture3D);
		return false;
	}

	[ServerCallback]
	public virtual bool BoundsCheck(NetworkConnectionToClient viewer, Vector3 hitPoint, float toleranceDistance, out float distance, out Vector3 nearest)
	{
		if (!NetworkServer.active)
		{
			distance = default(float);
			nearest = default(Vector3);
			return default(bool);
		}
		if (Sample(viewer, out var sample))
		{
			nearest = new Bounds(sample.position, sample.size).ClosestPoint(hitPoint);
			distance = Vector3.Distance(nearest, hitPoint);
			return distance <= toleranceDistance;
		}
		nearest = hitPoint;
		distance = 0f;
		return false;
	}

	[ServerCallback]
	public virtual bool RaycastCheck(NetworkConnectionToClient viewer, Vector3 originPoint, Vector3 hitPoint, float tolerancePercent, int layerMask, out RaycastHit hit)
	{
		if (!NetworkServer.active)
		{
			hit = default(RaycastHit);
			return default(bool);
		}
		if (Sample(viewer, out var sample))
		{
			GameObject gameObject = new GameObject("LagCompensatorTest");
			gameObject.transform.position = sample.position;
			gameObject.AddComponent<BoxCollider>().size = sample.size * (1f + tolerancePercent);
			Vector3 direction = hitPoint - originPoint;
			float maxDistance = direction.magnitude * 2f;
			bool result = Physics.Raycast(originPoint, direction, out hit, maxDistance, layerMask);
			Object.Destroy(gameObject);
			return result;
		}
		hit = default(RaycastHit);
		return false;
	}

	public override bool Weaved()
	{
		return true;
	}
}
