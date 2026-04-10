using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Splines;

public static class BoundsJobHelper
{
	public static bool DoesCurveRequiresSplittingForWindingAngle(float3 point, BezierCurve curve)
	{
		float x = math.length(curve.P0 - point);
		float y = math.length(curve.P3 - point);
		return curve.GetBoundingLength() >= math.max(x, y);
	}

	public static float GetCurveWindingAngleRad(float3 point, float3 curveStart, float3 curveEnd)
	{
		float2 vector = curveStart.xz - point.xz;
		float2 vector2 = curveEnd.xz - point.xz;
		float angleRad = vector.GetAngleRad();
		float num = (vector2.GetAngleRad() - angleRad).WrapAngleTauRad();
		if (!(num > MathF.PI))
		{
			return num;
		}
		return num - MathF.PI * 2f;
	}

	public static float GetBezierTotalWindingAngleRad(BezierCurve curve, float3 point)
	{
		NativeQueue<BezierCurve> nativeQueue = new NativeQueue<BezierCurve>(Allocator.Temp);
		nativeQueue.Enqueue(curve);
		float num = 0f;
		int num2 = 0;
		BezierCurve item;
		while (nativeQueue.TryDequeue(out item))
		{
			if (DoesCurveRequiresSplittingForWindingAngle(point, item))
			{
				CurveUtility.Split(item, 0.5f, out var left, out var right);
				nativeQueue.Enqueue(left);
				nativeQueue.Enqueue(right);
				continue;
			}
			num += GetCurveWindingAngleRad(point, item.P0, item.P3);
			if (num2++ > 100)
			{
				break;
			}
		}
		return num;
	}

	public static BoundsState IsInOutOfBoundsHazard(float3 point, NativeList<BoundsManager.SecondaryOutOfBoundsHazardInstance> secondaryHazards, float mainOutOfBoundsHazardHeight, OutOfBoundsHazard mainOutOfBoundsHazardHazardType, out float secondaryHazardHeight, out OutOfBoundsHazard hazardType, out int secondaryHazardInstanceId)
	{
		hazardType = (OutOfBoundsHazard)(-1);
		secondaryHazardInstanceId = 0;
		foreach (BoundsManager.SecondaryOutOfBoundsHazardInstance item in secondaryHazards)
		{
			if (IsInSecondaryHazardBounds(item))
			{
				secondaryHazardHeight = item.worldHeight;
				secondaryHazardInstanceId = item.instanceId;
				if (point.y > item.worldHeight)
				{
					return BoundsState.OverSecondaryOutOfBoundsHazard;
				}
				hazardType = item.type;
				return BoundsState.InSecondaryOutOfBoundsHazard;
			}
		}
		secondaryHazardHeight = mainOutOfBoundsHazardHeight;
		if (point.y <= mainOutOfBoundsHazardHeight)
		{
			hazardType = mainOutOfBoundsHazardHazardType;
			return BoundsState.InMainOutOfBoundsHazard;
		}
		return BoundsState.InBounds;
		bool IsInSecondaryHazardBounds(BoundsManager.SecondaryOutOfBoundsHazardInstance secondaryHazard)
		{
			if (point.x < secondaryHazard.worldHorizontalMin.x)
			{
				return false;
			}
			if (point.x > secondaryHazard.worldHorizontalMax.x)
			{
				return false;
			}
			if (point.z < secondaryHazard.worldHorizontalMin.y)
			{
				return false;
			}
			if (point.z > secondaryHazard.worldHorizontalMax.y)
			{
				return false;
			}
			return true;
		}
	}

	public static bool IsInOrOverLevelHazard(float3 point, NativeList<BoundsManager.LevelHazardInstance> hazards, out bool isOverHazard, out float hazardHeight, out LevelHazardType hazardType, out int levelHazardInstanceId)
	{
		hazardType = (LevelHazardType)(-1);
		levelHazardInstanceId = 0;
		foreach (BoundsManager.LevelHazardInstance item in hazards)
		{
			if (IsInLevelHazardBounds(item))
			{
				hazardHeight = item.worldHeight;
				levelHazardInstanceId = item.instanceId;
				hazardType = item.type;
				isOverHazard = point.y > item.worldHeight;
				return true;
			}
		}
		hazardHeight = float.MinValue;
		isOverHazard = false;
		return false;
		bool IsInLevelHazardBounds(BoundsManager.LevelHazardInstance secondaryHazard)
		{
			if (point.x < secondaryHazard.worldHorizontalMin.x)
			{
				return false;
			}
			if (point.x > secondaryHazard.worldHorizontalMax.x)
			{
				return false;
			}
			if (point.z < secondaryHazard.worldHorizontalMin.y)
			{
				return false;
			}
			if (point.z > secondaryHazard.worldHorizontalMax.y)
			{
				return false;
			}
			return true;
		}
	}
}
