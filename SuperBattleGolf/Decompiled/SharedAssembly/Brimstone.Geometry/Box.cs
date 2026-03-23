#define DEBUG_DRAW
using System.Collections.Generic;
using UnityEngine;

namespace Brimstone.Geometry;

public struct Box
{
	public Vector3 center;

	public Quaternion orientation;

	public Vector3 Size { get; private set; }

	public Vector3 HalfSize { get; private set; }

	public Vector3 Min { get; private set; }

	public Vector3 Max { get; private set; }

	public Box(Vector3 center, float size)
		: this(center, Vector3.one * size, Quaternion.identity)
	{
	}

	public Box(Vector3 center, Vector3 size)
		: this(center, size, Quaternion.identity)
	{
	}

	public Box(Vector3 center, float size, Quaternion orientation)
		: this(center, Vector3.one * size, orientation)
	{
	}

	public Box(Vector3 center, Vector3 size, Quaternion orientation)
	{
		this.center = center;
		Size = size;
		this.orientation = orientation;
		HalfSize = size * 0.5f;
		Min = center - HalfSize;
		Max = center + HalfSize;
	}

	public void SetSize(Vector3 size)
	{
		Size = size;
		HalfSize = size * 0.5f;
		Min = center - HalfSize;
		Max = center + HalfSize;
	}

	public void SetSize(float size)
	{
		Size = size * Vector3.one;
		HalfSize = Size * 0.5f;
		Min = center - HalfSize;
		Max = center + HalfSize;
	}

	public void SetMin(Vector3 min)
	{
		center = BMath.Average(min, Max);
		Size = BMath.Abs(Max - min);
		HalfSize = Size * 0.5f;
		Min = min;
	}

	public void SetMax(Vector3 max)
	{
		center = BMath.Average(Min, max);
		Size = BMath.Abs(max - Min);
		HalfSize = Size * 0.5f;
		Max = max;
	}

	public void SetMinAndMax(Vector3 min, Vector3 max)
	{
		center = BMath.Average(min, max);
		Size = BMath.Abs(max - min);
		HalfSize = Size * 0.5f;
		Min = min;
		Max = max;
	}

	public IEnumerable<Vector3> GetCorners()
	{
		Vector3 right = orientation * Vector3.right;
		Vector3 up = orientation * Vector3.up;
		Vector3 forward = orientation * Vector3.forward;
		yield return Min;
		yield return Max;
		yield return Min + Size.x * right;
		yield return Max - Size.x * right;
		yield return Max - Size.y * up;
		yield return Min + Size.y * up;
		yield return Min + Size.z * forward;
		yield return Max - Size.z * forward;
	}

	public bool Check(int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
	{
		return UnityEngine.Physics.CheckBox(center, HalfSize, orientation, layerMask, queryTriggerInteraction);
	}

	public Collider[] Overlap(int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
	{
		return UnityEngine.Physics.OverlapBox(center, HalfSize, orientation, layerMask, queryTriggerInteraction);
	}

	public int OverlapNonAlloc(int layerMask, Collider[] results, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
	{
		return UnityEngine.Physics.OverlapBoxNonAlloc(center, HalfSize, results, orientation, layerMask, queryTriggerInteraction);
	}

	public bool Cast(Vector3 direction, float maxDistance, int layerMask, out RaycastHit hitInfo, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
	{
		return UnityEngine.Physics.BoxCast(center, HalfSize, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public RaycastHit[] CastAll(Vector3 direction, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
	{
		return UnityEngine.Physics.BoxCastAll(center, HalfSize, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public int CastNonAlloc(Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
	{
		return UnityEngine.Physics.BoxCastNonAlloc(center, HalfSize, direction, results, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public void DrawDebug(Color color, float time = 0f)
	{
		BDebug.DrawWireCube(center, Size, orientation, color, time);
	}
}
