using System.Collections.Generic;
using UnityEngine;

public class BezierSpline : TrackablePath
{
	[SerializeField]
	private List<Transform> nodes = new List<Transform>();

	public const float SPLINE_NODE_RADIUS = 0.0625f;

	public List<Transform> Nodes
	{
		get
		{
			UpdateNodeList();
			return nodes;
		}
	}

	private void OnDrawGizmos()
	{
		if (nodes.Count == 0)
		{
			return;
		}
		_ = base.transform.rotation * base.transform.position;
		for (int i = 0; i < nodes.Count; i++)
		{
			Transform transform = nodes[i];
			Transform transform2 = ((i >= nodes.Count - 1) ? null : nodes[i + 1]);
			if ((bool)transform2)
			{
				Vector3 frontHandleWorldPos = GetFrontHandleWorldPos(transform);
				Vector3 backHandleWorldPos = GetBackHandleWorldPos(transform2);
				Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
				Gizmos.DrawWireSphere(transform.position, 0.0625f);
				Gizmos.DrawWireSphere(transform2.position, 0.0625f);
				Gizmos.DrawLine(transform.position, transform.position + transform.up.normalized * 0.5f);
				Gizmos.DrawLine(transform2.position, transform2.position + transform2.up.normalized * 0.5f);
				Gizmos.color = new Color(1f, 0.5f, 0.25f, 0.5f);
				Gizmos.DrawLine(transform.position, frontHandleWorldPos);
				Gizmos.DrawLine(transform2.position, backHandleWorldPos);
				Gizmos.DrawWireSphere(frontHandleWorldPos, 0.0625f);
				Gizmos.DrawWireSphere(backHandleWorldPos, 0.0625f);
			}
		}
	}

	private void OnValidate()
	{
		UpdateNodeList();
	}

	public void UpdateNodeList()
	{
		if (nodes == null)
		{
			nodes = new List<Transform>();
		}
		if (nodes.Count == base.transform.childCount)
		{
			return;
		}
		nodes = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			nodes.Add(item);
		}
	}

	public Vector3 GetFrontHandleWorldPos(Transform node)
	{
		return node.position + node.forward.normalized * node.localScale.z;
	}

	public Vector3 GetBackHandleWorldPos(Transform node)
	{
		return node.position + -node.forward.normalized * node.localScale.z;
	}

	public OrientedPoint GetOrientedPoint(float pointValue)
	{
		if (nodes.Count == 0)
		{
			return new OrientedPoint(base.transform.position, base.transform.rotation);
		}
		while (pointValue < 0f)
		{
			pointValue += (float)(nodes.Count - 1);
		}
		if (pointValue >= (float)(nodes.Count - 1))
		{
			Transform transform = nodes[nodes.Count - 1];
			return new OrientedPoint(transform.position, transform.rotation);
		}
		int num = Mathf.FloorToInt(pointValue);
		int index = num + 1;
		float interpolation = pointValue - (float)num;
		return GetOrientedPointOnCurve(nodes[num], nodes[index], interpolation);
	}

	private OrientedPoint GetOrientedPointOnCurve(Transform nodeA, Transform nodeB, float interpolation)
	{
		float num = Mathf.Pow(interpolation, 2f);
		float num2 = Mathf.Pow(interpolation, 3f);
		Vector3 position = nodeA.position;
		Vector3 position2 = nodeB.position;
		Vector3 frontHandleWorldPos = GetFrontHandleWorldPos(nodeA);
		Vector3 backHandleWorldPos = GetBackHandleWorldPos(nodeB);
		Vector3 position3 = position + interpolation * (-3f * position + 3f * frontHandleWorldPos) + num * (3f * position + -6f * frontHandleWorldPos + 3f * backHandleWorldPos) + num2 * (-position + 3f * frontHandleWorldPos + -3f * backHandleWorldPos + position2);
		Quaternion rotation = Quaternion.LookRotation(3f * (position2 + -3f * backHandleWorldPos + 3f * frontHandleWorldPos + -position) * num + 2f * (3f * backHandleWorldPos + -6f * frontHandleWorldPos + 3f * position) * interpolation + 3f * frontHandleWorldPos + -3f * position, Quaternion.Slerp(nodeA.rotation, nodeB.rotation, interpolation) * Vector3.up);
		return new OrientedPoint(position3, rotation);
	}

	public void AddSegment()
	{
		if (base.transform.childCount == 0)
		{
			AddNode(Vector3.zero, Quaternion.identity);
			AddNode(Vector3.right * 2f, Quaternion.identity);
		}
		else
		{
			UpdateNodeList();
			OrientedPoint orientedPoint = GetOrientedPoint(nodes.Count);
			AddNode(orientedPoint.position + (orientedPoint.rotation * Vector3.forward).normalized * 2f, orientedPoint.rotation);
		}
		UpdateNodeList();
	}

	private void AddNode(Vector3 localPosition, Quaternion localRotation)
	{
		GameObject obj = new GameObject("Node");
		obj.transform.SetParent(base.transform);
		obj.transform.localPosition = localPosition;
		obj.transform.localRotation = localRotation;
		obj.transform.localScale = Vector3.one;
	}

	public void ConnectTo(Transform node)
	{
		if (nodes.Count == 0)
		{
			return;
		}
		Vector3 localPosition = nodes[0].localPosition;
		if (localPosition != Vector3.zero)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].localPosition = nodes[i].localPosition - localPosition;
			}
		}
		Transform parent = base.transform.parent;
		Vector3 localScale = base.transform.localScale;
		base.transform.parent = node;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.parent = parent;
		base.transform.localScale = localScale;
		Quaternion quaternion = node.rotation * Quaternion.Inverse(nodes[0].rotation);
		if (quaternion != Quaternion.identity)
		{
			base.transform.localRotation = base.transform.localRotation * quaternion;
		}
	}

	public override OrientedPoint GetTrackingPoint(SplineTrackingMode mode, float trackingValue)
	{
		if (nodes.Count == 0)
		{
			return new OrientedPoint(base.transform.position, base.transform.rotation);
		}
		switch (mode)
		{
		case SplineTrackingMode.Point:
			return GetOrientedPoint(trackingValue % (float)(nodes.Count - 1));
		case SplineTrackingMode.Normalized:
		{
			float trackingValue2 = (float)(nodes.Count - 1) * (trackingValue % 1f);
			return GetTrackingPoint(SplineTrackingMode.Point, trackingValue2);
		}
		default:
			return new OrientedPoint(base.transform.position, base.transform.rotation);
		}
	}
}
